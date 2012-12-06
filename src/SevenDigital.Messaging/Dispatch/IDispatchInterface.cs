using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDispatchInterface:IDisposable
	{
		void SubscribeHandler<T>(Action<T> action, string destinationName)where T: class;
		void Publish<T>(T message);
	}
}