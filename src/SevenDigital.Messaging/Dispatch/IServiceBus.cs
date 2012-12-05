using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IServiceBus
	{
		void SubscribeHandler<T>(Action<T> action);
		void Publish<T>(T message);
		void Dispose();
	}
}