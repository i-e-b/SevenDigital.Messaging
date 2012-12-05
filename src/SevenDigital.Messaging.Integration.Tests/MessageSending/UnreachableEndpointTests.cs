using System;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class UnreachableEndpointTests
	{
		[SetUp]
		public void Messaging_configured_to_point_at_an_unreachable_server ()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer("example.com");
		}

		[Test]
		public void Sending_message_should_try_for_at_least_60_seconds ()
		{
			ObjectFactory.Configure(map=>map.For<IServiceBusFactory>().Use<FakeSBF>());

			var sender = ObjectFactory.GetInstance<ISenderNode>();
			var start = DateTime.Now;

			try
			{
				sender.SendMessage(new GreenMessage());
			} catch
			{
				Console.WriteLine("Final fail");
			}

			var time = DateTime.Now - start;
			Assert.That(time.TotalSeconds, Is.GreaterThan(60));
		}
	}

	public class FakeSBF:IServiceBusFactory
	{
		public IServiceBus Create(Uri address)
		{
			throw new RabbitMQ.Client.Exceptions.BrokerUnreachableException(null,null);
		}
	}
}
