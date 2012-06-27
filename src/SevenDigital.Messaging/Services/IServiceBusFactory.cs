using System;
using MassTransit;

namespace SevenDigital.Messaging.Core.Services
{
	public interface IServiceBusFactory
	{
		IServiceBus Create(Uri address);
	}
}