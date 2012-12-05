using System;

namespace SevenDigital.Messaging.Dispatch
{
	public class DummyStub__ServiceBusFactory: IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return null;
		}
	}
}