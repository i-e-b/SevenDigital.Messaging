using System;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging
{
	public class DummyStub__ServiceBusFactory: IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			return null;
		}
	}
}