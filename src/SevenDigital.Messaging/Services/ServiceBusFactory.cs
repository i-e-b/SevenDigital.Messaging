using System;
using MassTransit;

namespace SevenDigital.Messaging.Core.Services
{
	public class ServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return MassTransit.ServiceBusFactory.New(bus =>
			{
				bus.ReceiveFrom(address);
				bus.UseHealthMonitoring(10);
				bus.UseRabbitMqRouting();
				bus.UseControlBus();
			});
		}
	}
}