using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
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
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(HostName);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			ObjectFactory.Container.Dispose();
		}

		[Test]
		public void Should_have_sleep_wrapper()
		{
			Assert.That(ObjectFactory.GetInstance<ISleepWrapper>(), Is.InstanceOf<SleepWrapper>());
		}

		[Test]
		public void Should_configure_messaging_base()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingBase>(), Is.Not.Null);
		}

		[Test]
		public void Should_get_messaging_host_implementation()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>(), Is.Not.Null);
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>(), Is.InstanceOf<Host>());
		}

		[Test]
		public void Should_give_provided_name_as_host_string()
		{
			Assert.That(ObjectFactory.GetInstance<IMessagingHost>().ToString(),
				Is.EqualTo(HostName));
		}

		[Test]
		public void Should_have_unique_name_generator_instance()
		{
			Assert.That(ObjectFactory.GetInstance<IUniqueEndpointGenerator>(), Is.InstanceOf<UniqueEndpointGenerator>());
		}

		[Test]
		public void Should_get_no_event_hook_implementations_by_default()
		{
			Assert.That(ObjectFactory.GetAllInstances<IEventHook>(), Is.Empty);
		}

		[Test]
		public void Should_be_able_to_set_and_clear_event_hooks()
		{
			MessagingSystem.Events.AddEventHook<DummyEventHook>();
			Assert.That(ObjectFactory.GetInstance<IEventHook>(), Is.InstanceOf<DummyEventHook>());

			MessagingSystem.Events.ClearEventHooks();
			Assert.That(ObjectFactory.TryGetInstance<IEventHook>(), Is.Null);
		}

		[Test]
		public void Should_get_node_factory_implementation()
		{
			var factory = MessagingSystem.Receiver();
			Assert.That(factory, Is.Not.Null);
			Assert.That(factory, Is.InstanceOf<IReceiver>());
		}

		[Test]
		public void Should_get_sender_node_implementation()
		{
			var sender = MessagingSystem.Sender();
			Assert.That(sender, Is.InstanceOf<SenderNode>());
			Assert.That(sender, Is.Not.Null);
		}
	}
}
