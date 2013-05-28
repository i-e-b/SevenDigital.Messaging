using System;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard sender node for Messaging.
	/// You do not need to create this yourself. Use `Messaging.Sender()`
	/// </summary>
	public class SenderNode : ISenderNode
	{
		readonly IMessagingBase messagingBase;

		/// <summary>
		/// Create a new message sending node. You do not need to create this yourself. Use `Messaging.Sender()`
		/// </summary>
		public SenderNode(IMessagingBase messagingBase)
		{
			this.messagingBase = messagingBase;
		}

		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		public virtual void SendMessage<T>(T message) where T : class, IMessage
		{
			TryFireHooks(message);
			TrySendMessage(message);
		}

		static void TryFireHooks<T>(T message) where T : class, IMessage
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

		void TrySendMessage<T>(T message) where T : class, IMessage
		{
			messagingBase.SendMessage(message);
				
		}
	}
}