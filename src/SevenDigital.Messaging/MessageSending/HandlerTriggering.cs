using System;
using MassTransit;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class HandlerTriggering<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly IServiceBus serviceBus;

		public HandlerTriggering(IServiceBus serviceBus)
		{
			this.serviceBus = serviceBus;
		}

		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			serviceBus.SubscribeHandler<TMessage>(msg =>
			{
				try
				{
					ObjectFactory.GetInstance<THandler>().Handle(msg);

					ObjectFactory
						.GetAllInstances<IEventHook>()
						.ForEach(hook => hook.MessageReceived(msg));
				}
				catch (Exception ex)
				{
					ObjectFactory
					.GetAllInstances<IEventHook>()
					.ForEach(hook => hook.HandlerFailed(msg, typeof(THandler), ex));
				}
			});
		}
	}
}