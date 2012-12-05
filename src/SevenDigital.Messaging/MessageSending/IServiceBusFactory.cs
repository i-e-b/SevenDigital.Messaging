using System;
using MassTransit;

namespace SevenDigital.Messaging.MessageSending
{
	public interface IServiceBusFactory
	{
		IServiceBus Create(Uri address);
	}
}