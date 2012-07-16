using System;
using MassTransit;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class MessageBinding<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly IServiceBus serviceBus;

		public MessageBinding(IServiceBus serviceBus)
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