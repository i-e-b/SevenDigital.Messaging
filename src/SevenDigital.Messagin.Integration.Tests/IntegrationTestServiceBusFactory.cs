using System;
using MassTransit;
using SevenDigital.Messaging.Core.Services;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class IntegrationTestServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return MassTransit.ServiceBusFactory.New(bus =>
			{
				bus.ReceiveFrom(address);
				bus.UseHealthMonitoring(10);
				bus.UseRabbitMqRouting();
				bus.SetPurgeOnStartup(true);	//this ensures the queues are cleared out each time.
			});
		}
	}
}