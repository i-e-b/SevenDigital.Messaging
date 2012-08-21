using System;
using MassTransit;

namespace SevenDigital.Messaging.MessageSending
{
	public class MessagePurgingServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return MassTransit.ServiceBusFactory.New(bus =>
			{
				bus.ReceiveFrom(address);
				bus.UseRabbitMqRouting();
				bus.SetPurgeOnStartup(true);	//this ensures the queues are cleared out each time.
			});
		}
	}
}