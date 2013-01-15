using System;
using System.Threading;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;

namespace SevenDigital.Messaging.Dispatch
{
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

		public DestinationPoller(IMessagingBase messagingBase, ISleepWrapper sleeper, IMessageDispatcher dispatcher)
		{
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
				object message = null;
				if (dispatcher.HandlersInflight < TaskLimit) message = GetMessageRobust();
				if (message != null)
				{
					dispatcher.TryDispatch(message);
					sleep = 0;
				}
				else
				{
					sleeper.Sleep(sleep);
					sleep = burstSleep(sleep);
				}
			}
		}

		int burstSleep(int sleep)
		{
			switch (sleep)
			{
				case 0: return 1;
				case 1: return 125;
				default: return 500;
			}
		}

		IMessage GetMessageRobust()
		{
			try
			{
				return messagingBase.GetMessage<IMessage>(destination);
			}
			catch (Exception ex)
			{
				Log.Warning("Could not pick up message because "+ex.GetType().Name+": "+ex.Message);
				return null;
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

		public void AddHandler<T>(Action<T> action)
		{
			dispatcher.AddHandler(action);
		}
	}
}