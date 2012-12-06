using System;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatch:IMessageDispatch
	{
		readonly IMessagingBase messagingBase;

		public MessageDispatch(IMessagingBase messagingBase)
		{
			this.messagingBase = messagingBase;
		}

		public void Publish<T>(T message)
		{
			messagingBase.SendMessage(message);
		}

		public void Dispose()
		{
		}

		public void SubscribeHandler<T>(Action<T> action)
		{
		}
	}
}
