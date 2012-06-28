using System;
using MassTransit;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
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
			{
				try
				{
					ObjectFactory.GetInstance<THandler>().Handle(msg);
				}
				catch (Exception ex)
				{
					ObjectFactory
					.GetAllInstances<IEventHook>()
					.ForEach(hook => hook.HandlerFailed(msg, typeof(THandler), ex));

					throw;
				}

				ObjectFactory
					.GetAllInstances<IEventHook>()
					.ForEach(hook => hook.MessageReceived(msg));
			});
		}
	}
}