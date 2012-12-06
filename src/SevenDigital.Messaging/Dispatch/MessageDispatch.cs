using System;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatch:IMessageDispatch
	{
		readonly IMessagingBase messagingBase;
		readonly IDestinationPoller destinationPoller;
		readonly IMessageDispatcher messageDispatcher;

		public MessageDispatch(IMessagingBase messagingBase, IDestinationPoller destinationPoller, IMessageDispatcher messageDispatcher)
		{
			this.messagingBase = messagingBase;
			this.destinationPoller = destinationPoller;
			this.messageDispatcher = messageDispatcher;
		}

		public void Publish<T>(T message)
		{
			messagingBase.SendMessage(message);
		}

		public void Dispose()
		{
		}

		public void SubscribeHandler<T>(Action<T> action, string destinationName)
		{
			messageDispatcher.AddHandler(action);
			destinationPoller.AddDestinationToWatch(destinationName);
			destinationPoller.Start();
		}
	}
}
