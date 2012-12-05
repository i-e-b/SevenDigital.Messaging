using System;

namespace SevenDigital.Messaging.MessageSending
{
	public interface IServiceBusFactory
	{
		IServiceBus Create(Uri address);
	}
}