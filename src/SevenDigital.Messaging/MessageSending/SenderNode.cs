using System;
using System.Collections.Generic;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using DispatchSharp.WorkerPools;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.MessageReceiving;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard sender node for Messaging.
	/// You do not need to create this yourself. Use `Messaging.Sender()`
	/// </summary>
	public class SenderNode : ISenderNode
	{
		const int SingleThreaded = 1;
		readonly IMessagingBase _messagingBase;
		readonly ISleepWrapper _sleeper;
		readonly IDispatch<IMessage> _sendingDispatcher;

		/// <summary>
		/// Create a new message sending node. You do not need to create this yourself. Use `Messaging.Sender()`
		/// </summary>
		public SenderNode(IMessagingBase messagingBase, IDispatcherFactory dispatchFactory, ISleepWrapper sleeper)
		{


			_messagingBase = messagingBase;
			_sleeper = sleeper;
			_sendingDispatcher = dispatchFactory.Create( 
				new InMemoryWorkQueue<IMessage>(), // later, replace with Persistent Disk Queue
				new ThreadedWorkerPool<IMessage>("SDMessaging_Sender", SingleThreaded)
			);

			_sendingDispatcher.AddConsumer(SendWaitingMessage);
			_sendingDispatcher.Start();
			_sendingDispatcher.Exceptions += _sendingDispatcher_Exceptions;
		}

		void _sendingDispatcher_Exceptions(object sender, ExceptionEventArgs e)
		{
			 Log.Warning("Sender failed: " + e.SourceException.GetType() + "; " + e.SourceException.Message);
		}

		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		public virtual void SendMessage<T>(T message) where T : class, IMessage
		{
			_sendingDispatcher.AddWork(message);
		}

		/// <summary>
		/// Internal immediate send. Use SendMessage() instead.
		/// </summary>
		public void SendWaitingMessage(IMessage message)
		{
			TryFireHooks(message);
			try
			{
				_messagingBase.SendMessage(message);
				_sleeper.Reset();
			}
			catch
			{
				_sleeper.SleepMore();
				SendMessage(message);
			}
		}

		static void TryFireHooks(IMessage message)
		{
			var hooks = GetEventHooks();

			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageSent(message);
				}
				catch (Exception ex)
				{
					Log.Warning("An event hook failed during send " + ex.GetType() + "; " + ex.Message);
				}
			}
		}

		static IEnumerable<IEventHook> GetEventHooks()
		{
			try
			{
				return ObjectFactory.GetAllInstances<IEventHook>();
			}
			catch (Exception ex)
			{
				Log.Warning("Structuremap could not generate event hook list " + ex.GetType() + "; " + ex.Message);
				return new IEventHook[0];
			}
		}

		/// <summary>
		/// Shutdown the sender
		/// </summary>
		public void Dispose()
		{
			_sendingDispatcher.Stop();
		}
	}
}