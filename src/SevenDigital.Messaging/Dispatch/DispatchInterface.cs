using System;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class DispatchInterface:IDispatchInterface
	{
		readonly IMessagingBase messagingBase;
		readonly IDestinationPoller destinationPoller;
		readonly IMessageDispatcher messageDispatcher;

		public DispatchInterface(IMessagingBase messagingBase, IDestinationPoller destinationPoller, IMessageDispatcher messageDispatcher)
		{
			this.messagingBase = messagingBase;
			this.destinationPoller = destinationPoller;
			this.messageDispatcher = messageDispatcher;
		}

		public void Publish<T>(T message)
		{
			messagingBase.SendMessage(message);
		}

	}
}
