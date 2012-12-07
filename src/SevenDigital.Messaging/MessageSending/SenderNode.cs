using System;
using System.Linq;
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
			string serialised = "";
			for (int i = 0; i < 5; i++)
			{
				try
				{
					serialised = messagingBase.SendMessage(message);
					break;
				} catch (Exception ex) {
					Console.WriteLine("Could not send message: "+ex.GetType()+": "+ex.Message);
					if (i == 4) throw;
				}
			}

			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageSent(message, serialised, ContractTypeName(message));
				}
				catch (Exception ex)
				{
					Console.WriteLine("An event hook failed during send: " + ex.GetType() + "; " + ex.Message);
				}
			}

		}

		static string ContractTypeName<T>(T message) where T : class, IMessage
		{
			return message.DirectlyImplementedInterfaces().Single().ToString();
		}
	}
}