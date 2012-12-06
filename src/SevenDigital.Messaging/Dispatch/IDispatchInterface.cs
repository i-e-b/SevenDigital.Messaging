using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDispatchInterface
	{
		void Publish<T>(T message);
	}
}