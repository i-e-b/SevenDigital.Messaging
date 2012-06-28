using System;
using NUnit.Framework;
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
		public void Should_get_no_event_hook_implementations_by_default ()
		{
			Assert.That(ObjectFactory.GetAllInstances<IEventHook>(), Is.Empty);
		}

		[Test]
		public void Should_be_able_to_set_and_clear_event_hooks ()
		{
			new MessagingConfiguration().WithEventHook<DummyEventHook>();
			Assert.That(ObjectFactory.GetInstance<IEventHook>(), Is.InstanceOf<DummyEventHook>());

			new MessagingConfiguration().ClearEventHooks();
			Assert.That(ObjectFactory.TryGetInstance<IEventHook>(), Is.Null);
		}

		[Test]
		public void Should_get_node_factory_implementation ()
		{
			var factory = ObjectFactory.GetInstance<INodeFactory>();
			Assert.That(factory, Is.Not.Null);
		}

	}

	public class DummyEventHook:IEventHook
	{
		public void MessageSent(IMessage msg){}
		public void MessageReceived(IMessage msg){}
		public void HandlerFailed(IMessage message, Type handler, Exception ex){}
	}
}
