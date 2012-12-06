using System;
using SevenDigital.Messaging.Base;
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
			var hooks = ObjectFactory.GetAllInstances<IEventHook>();

			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageSent(message);
				}
				catch (Exception ex)
				{
					Console.WriteLine("An event hook failed during send: " + ex.GetType() + "; " + ex.Message);
				}
			}

			for (int i = 0; i < 3; i++)
			{
				try
				{
					messagingBase.SendMessage(message);
					break;
				} catch (Exception ex) {
					Console.WriteLine("Could not send message: "+ex.GetType()+": "+ex.Message);
				}
			}
		}
	}
}