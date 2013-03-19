using System;
using System.Collections.Generic;
using System.Threading;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Destination poller is bound to a single destination queue
	/// and keeps a list of message types that have been registered.
	/// 
	/// When a message is available on the queue, the Message Dispatcher
	/// is called to run all the handlers for the received message type.
	/// </summary>
	public class DestinationPoller : IDestinationPoller
	{
		string destination;
		readonly IMessagingBase messagingBase;
		readonly ISleepWrapper sleeper;
		readonly IMessageDispatcher dispatcher;
		Thread pollingThread;
		int runningExch;
		volatile bool running;
		internal static volatile int TaskLimit = 4;
		readonly HashSet<Type> _boundMessageTypes;

		public DestinationPoller(IMessagingBase messagingBase, ISleepWrapper sleeper, IMessageDispatcher dispatcher)
		{
			_boundMessageTypes = new HashSet<Type>();
			this.messagingBase = messagingBase;
			this.sleeper = sleeper;
			this.dispatcher = dispatcher;
		}

		public void SetDestinationToWatch(string targetDestination)
		{
			destination = targetDestination;
		}

		public void PollingMethod()
		{
			var sleep = 0;
			while (running)
			{
				IPendingMessage<object> message = null;
				if (dispatcher.HandlersInflight < TaskLimit) message = GetMessageRobust();
				if (message != null)
				{
					dispatcher.TryDispatch(message);
					sleep = 0;
				}
				else
				{
					sleeper.Sleep(sleep);
					sleep = BurstSleep(sleep);
				}
			}
		}

		static int BurstSleep(int sleep)
		{
			if (sleep < 255) return (sleep * 2) + 1;
			return 255;
		}

		IPendingMessage<IMessage> GetMessageRobust()
		{
			try
			{
				return messagingBase.TryStartMessage<IMessage>(destination);
			}
			catch (Exception ex)
			{
				if (IsMissingQueue(ex)) TryRebuildQueues();
				Log.Warning("Could not pick up message because " + ex.GetType().Name + ": " + ex.Message);
				return null;
			}
		}

		static bool IsMissingQueue(Exception exception)
		{
			var e = exception as RabbitMQ.Client.Exceptions.OperationInterruptedException;
			return (e != null)
				&& (e.ShutdownReason.ReplyCode == 404);
		}

		void TryRebuildQueues()
		{
			MessagingBase.ResetCaches();
			foreach (var sourceMessage in _boundMessageTypes)
			{
				messagingBase.CreateDestination(sourceMessage, destination);
			}
		}

		public void Start()
		{
			lock (this)
			{
				if (!TryStartRunning()) return;

				if (pollingThread == null) pollingThread = new Thread(PollingMethod)
					{
						Name = "Polling_" + destination,
						IsBackground = true
					};
				if (pollingThread.ThreadState != ThreadState.Running) pollingThread.Start();
			}
		}

		public bool TryStartRunning()
		{
			var original = Interlocked.CompareExchange(ref runningExch, -1, 0);
			if (original == 0)
			{
				running = true;
				return true;
			}
			return false;
		}

		public void StopRunning()
		{
			running = false;
			runningExch = 0;
		}

		public void Stop()
		{
			StopPollingThread();
			WaitForHandlersToFinish();
		}

		void WaitForHandlersToFinish()
		{
			while (dispatcher.HandlersInflight > 0)
			{
				sleeper.Sleep(100);
			}
		}

		void StopPollingThread()
		{
			StopRunning();
			var pt = pollingThread;
			pollingThread = null;
			if (pt != null)
			{
				pt.Join();
			}
		}

		public void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>
		{
			AddMessageType(typeof(TMessage));
			dispatcher.AddHandler<TMessage, THandler>();
		}

		void AddMessageType(Type type)
		{
			lock (_boundMessageTypes)
			{
				_boundMessageTypes.Add(type);
			}
		}

		public void RemoveHandler<THandler>()
		{
			dispatcher.RemoveHandler<THandler>();
		}

		public int HandlerCount
		{
			get
			{
				return dispatcher.CountHandlers();
			}
		}
	}
}