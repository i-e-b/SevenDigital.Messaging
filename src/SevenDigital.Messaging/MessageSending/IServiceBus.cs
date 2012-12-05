using System;

namespace SevenDigital.Messaging.MessageSending
{
	public interface IServiceBus
	{
		void SubscribeHandler<T>(Action<T> action);
		void Publish<T>(T message);
		void Dispose();
	}
}