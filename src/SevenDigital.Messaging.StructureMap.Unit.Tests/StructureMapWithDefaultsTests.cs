using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class StructureMapWithDefaultsTests
	{
		const string HostName = "my.unique.host";

		[TestFixtureSetUp]
		public void Setup()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer(HostName);
		}

        [TestFixtureTearDown]
        public void TearDown()
        {
            ObjectFactory.Container.Dispose();
        }

		[Test]
		public void Should_get_messaging_host_implementation ()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>(), Is.Not.Null);
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>(), Is.InstanceOf<Host>());
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
			Assert.That(ObjectFactory.GetInstance<IServiceBusFactory>(), Is.InstanceOf<DummyStub__ServiceBusFactory>());
		}

		[Test]
		public void Should_get_no_event_hook_implementations_by_default ()
		{
			Assert.That(ObjectFactory.GetAllInstances<IEventHook>(), Is.Empty);
		}

		[Test]
		public void Should_be_able_to_set_and_clear_event_hooks ()
		{
			new MessagingConfiguration().AddEventHook<DummyEventHook>();
			Assert.That(ObjectFactory.GetInstance<IEventHook>(), Is.InstanceOf<DummyEventHook>());

			new MessagingConfiguration().ClearEventHooks();
			Assert.That(ObjectFactory.TryGetInstance<IEventHook>(), Is.Null);
		}

		[Test]
		public void Should_get_node_factory_implementation ()
		{
			var factory = ObjectFactory.GetInstance<INodeFactory>();
			Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.InstanceOf<NodeFactory>());
		}

        [Test]
        public void Should_get_sender_node_implementation()
        {
            var senderNode = ObjectFactory.GetInstance<ISenderNode>();
            Assert.That(senderNode, Is.InstanceOf<SenderNode>());
            Assert.That(senderNode, Is.Not.Null);
        }
	}
}
