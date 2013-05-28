using System;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class SenderNode : ISenderNode
	{
		readonly IMessagingBase messagingBase;

		public SenderNode(IMessagingBase messagingBase)
		{
			this.messagingBase = messagingBase;
		}

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