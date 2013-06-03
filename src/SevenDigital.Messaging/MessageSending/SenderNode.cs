using System;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using DispatchSharp.WorkerPools;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard sender node for Messaging.
	/// You do not need to create this yourself. Use `Messaging.Sender()`
	/// </summary>
	public class SenderNode : ISenderNode, IDisposable
	{
		const int SingleThreaded = 1;
		readonly IMessagingBase _messagingBase;
		readonly IDispatch<IMessage> _sendingDispatcher;

		/// <summary>
		/// Create a new message sending node. You do not need to create this yourself. Use `Messaging.Sender()`
		/// </summary>
		public SenderNode(IMessagingBase messagingBase)
		{
			_messagingBase = messagingBase;
			_sendingDispatcher = new Dispatch<IMessage>( 
				new InMemoryWorkQueue<IMessage>(), // later, replace with Persistent Disk Queue
				new ThreadedWorkerPool<IMessage>("SDMessaging_Sender", SingleThreaded)
				);

			_sendingDispatcher.AddConsumer(SendWaitingMessage);
			_sendingDispatcher.Start();
		}

		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		public virtual void SendMessage<T>(T message) where T : class, IMessage
		{
			_sendingDispatcher.AddWork(message);
		}

		void SendWaitingMessage(IMessage message)
		{
			try
			{
				TryFireHooks(message);
				_messagingBase.SendMessage(message);
			} catch
			{
				Console.WriteLine("WTF?");
			}
		}

		static void TryFireHooks(IMessage message)
		{
			var hooks = ObjectFactory.GetAllInstances<IEventHook>();
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

		public void Dispose()
		{
			_sendingDispatcher.Stop();
		}
	}
}