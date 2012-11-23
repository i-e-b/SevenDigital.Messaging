using System;
using MassTransit;

namespace SevenDigital.Messaging.MessageSending
{
	public class ServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return MassTransit.ServiceBusFactory.New(bus =>
			{
				bus.ReceiveFrom(address);
				bus.UseHealthMonitoring(100);
				bus.UseRabbitMqRouting();
			});
		}
	}
}