using MassTransit;
using StructureMap;

namespace SevenDigital.Messaging.Services
{
	public class MessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly IServiceBus _serviceBus;

		public MessageBinding(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			_serviceBus.SubscribeHandler<TMessage>(msg =>
				ObjectFactory.GetInstance<THandler>().Handle(msg)
				);
		}
	}
}