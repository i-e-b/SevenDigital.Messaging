using System;
using MassTransit;

namespace SevenDigital.Messaging.Services
{
	public interface IServiceBusFactory
	{
		IServiceBus Create(Uri address);
	}
}