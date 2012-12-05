using System;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging
{
	public class DummyStub__MessagePurgingServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return null;
		}
	}
}