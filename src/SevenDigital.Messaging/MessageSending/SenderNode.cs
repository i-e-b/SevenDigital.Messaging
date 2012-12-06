using System;
using SevenDigital.Messaging.Dispatch;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class SenderNode : ISenderNode
	{
		readonly IDispatchInterface dispatchInterface;

		public SenderNode(IDispatchInterface dispatchInterface)
		{
			this.dispatchInterface = dispatchInterface;
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
					dispatchInterface.Publish(message);
					break;
				} catch (Exception ex) {
					Console.WriteLine("Could not send message: "+ex.GetType()+": "+ex.Message);
				}
			}
		}
	}
}