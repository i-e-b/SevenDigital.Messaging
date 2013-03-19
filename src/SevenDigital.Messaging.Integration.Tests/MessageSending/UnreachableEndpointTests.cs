using System;
using NUnit.Framework;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture, Explicit]
	public class UnreachableEndpointTests
	{
		[SetUp]
		public void Messaging_configured_to_point_at_an_unreachable_server()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer("complete rubbish!");
		}

		[TearDown]
		public void teardown()
		{
			new MessagingConfiguration().Shutdown();
			ObjectFactory.EjectAllInstancesOf<ISleepWrapper>();
			ObjectFactory.EjectAllInstancesOf<IMessagingHost>();
			ObjectFactory.EjectAllInstancesOf<IRabbitMqConnection>();
			ObjectFactory.Configure(map => map.For<ISleepWrapper>().Use<SleepWrapper>());
		}

		[Test]
		public void Sending_message_should_try_for_at_least_60_seconds()
		{
			var countingWrapper = new CountingSleepWrapper();
			ObjectFactory.Configure(map => map.For<ISleepWrapper>().Use(countingWrapper));
			var sender = ObjectFactory.GetInstance<ISenderNode>();
			var start = DateTime.Now;

			try
			{
				sender.SendMessage(new GreenMessage());
			}
			catch
			{
				Console.WriteLine("Final fail");
			}

			var time = DateTime.Now - start;
			Assert.That(countingWrapper.total, Is.GreaterThanOrEqualTo(60000));
		}
	}

	public class CountingSleepWrapper : ISleepWrapper
	{
		public long total = 0;

		public void Reset()
		{
		}

		public void SleepMore()
		{
			total++;
		}
	}
}
