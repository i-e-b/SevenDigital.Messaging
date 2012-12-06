using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;
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
		public void Should_have_sleep_wrapper ()
		{
			Assert.That(ObjectFactory.GetInstance<ISleepWrapper>(), Is.InstanceOf<SleepWrapper>());
		}

		[Test]
		public void Should_have_thread_pool_wrapper ()
		{
			Assert.That(ObjectFactory.GetInstance<IThreadPoolWrapper>(), Is.InstanceOf<ThreadPoolWrapper>());
		}

		[Test]
		public void Should_have_destination_poller_as_singleton ()
		{
			var instance1 = ObjectFactory.GetInstance<IDestinationPoller>();
			var instance2 = ObjectFactory.GetInstance<IDestinationPoller>();

			Assert.That(instance1, Is.InstanceOf<DestinationPoller>());
			Assert.That(instance1, Is.SameAs(instance2));
		}

		[Test]
		public void Should_have_handler_dispatcher_as_singleton ()
		{
			var instance1 = ObjectFactory.GetInstance<IMessageDispatcher>();
			var instance2 = ObjectFactory.GetInstance<IMessageDispatcher>();

			Assert.That(instance1, Is.InstanceOf<MessageDispatcher>());
			Assert.That(instance1, Is.SameAs(instance2));
		}

		[Test]
		public void Should_have_node_implementation ()
		{
			Assert.That(ObjectFactory.GetInstance<INode>(), Is.InstanceOf<Node>());
		}

		[Test]
		public void Should_configure_messaging_base ()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingBase>(), Is.Not.Null);
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
