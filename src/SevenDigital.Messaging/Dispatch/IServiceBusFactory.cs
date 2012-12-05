using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IServiceBusFactory
	{
		IServiceBus Create(Uri address);
	}
}