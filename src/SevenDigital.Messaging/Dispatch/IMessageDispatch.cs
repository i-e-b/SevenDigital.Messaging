using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageDispatch:IDisposable
	{
		void SubscribeHandler<T>(Action<T> action);
		void Publish<T>(T message);
	}
}