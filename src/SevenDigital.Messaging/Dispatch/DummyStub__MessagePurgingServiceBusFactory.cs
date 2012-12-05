using System;

namespace SevenDigital.Messaging.Dispatch
{
	public class DummyStub__MessagePurgingServiceBusFactory : IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return null;
		}
	}
}