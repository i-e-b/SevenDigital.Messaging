using MassTransit;
using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Services
{
	public class MessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly IServiceBus _serviceBus;

		public MessageBinding(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		public void With<THandler>() where THandler : IHandle<TMessage>, new()
		{
			_serviceBus.SubscribeHandler<TMessage>(msg => new THandler().Handle(msg));
		}
	}
}