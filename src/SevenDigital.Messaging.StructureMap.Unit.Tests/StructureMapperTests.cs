using NUnit.Framework;
using SevenDigital.Messaging.EventStoreHooks;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class StructureMapperTests
	{
		const string HostName = "my.unique.host";

		[SetUp]
		public void Setup()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer(HostName);
		}

		[Test]
		public void Should_get_messaging_host_implementation ()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>(), Is.Not.Null);
		}

		[Test]
		public void Should_give_provided_name_as_host_string ()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>().ToString(),
				Is.EqualTo(HostName));
		}

		[Test]
		public void Should_have_unique_name_generator_instance ()
		{
			Assert.That(ObjectFactory.GetInstance<IUniqueEndpointGenerator>(), Is.InstanceOf<UniqueEndpointGenerator>());
		}

		[Test]
		public void Should_have_sender_name_generator_instance ()
		{
			Assert.That(ObjectFactory.GetInstance<ISenderEndpointGenerator>(), Is.InstanceOf<SenderEndpointGenerator>());
		}

		[Test]
		public void Should_have_service_bus_factory_instance ()
		{
			Assert.That(ObjectFactory.GetInstance<IServiceBusFactory>(), Is.InstanceOf<ServiceBusFactory>());
		}

		[Test]
		public void Should_get_NoEventStoreHook_implementation ()
		{
			Assert.That(ObjectFactory.GetInstance<IEventStoreHook>(), Is.InstanceOf<NoEventStoreHook>());
		}

		[Test]
		public void Should_get_node_factory_implementation ()
		{
			var factory = ObjectFactory.GetInstance<INodeFactory>();
			Assert.That(factory, Is.Not.Null);
		}

	}
}
