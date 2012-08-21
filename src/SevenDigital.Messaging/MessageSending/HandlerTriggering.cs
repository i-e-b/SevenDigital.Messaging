using System;
using System.Collections.Generic;
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
					Get<IHandle<TMessage>>(typeof(THandler)).Handle(msg);

					var hooks = GetAll<IEventHook>(typeof(IEventHook));
					foreach (var hook in hooks) {
						hook.MessageReceived(msg);
					}
				}
				catch (Exception ex)
				{
					var hooks = GetAll<IEventHook>(typeof(IEventHook));
					foreach (var hook in hooks) {
						hook.HandlerFailed(msg, typeof(THandler), ex);
					}
				}
			});
		}

		private static T Get<T>(Type src) {
			return (T)ObjectFactory.GetInstance(src);
		}
		
		private static IEnumerable<T> GetAll<T>(Type src) {
			return (IEnumerable<T>)ObjectFactory.GetAllInstances(src);
		}
	}
}