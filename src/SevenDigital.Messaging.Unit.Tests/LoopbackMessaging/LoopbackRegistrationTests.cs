using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
	[TestFixture]
	public class LoopbackRegistrationTests
	{
	    [SetUp]
		public void When_configuring_with_loopback_even_if_default_configuration_used ()
		{
			new MessagingConfiguration().WithLoopback();

            ObjectFactory.GetInstance<INodeFactory>().Listen().Handle<IMessage>().With<AHandler>();
		}

		[Test]
		public void Should_get_registered_listener_for_type ()
		{
			var listeners = new MessagingConfiguration().LoopbackListenersForMessage<IMessage>();

			Assert.That(listeners, Contains.Item(typeof(AHandler)));
		}

	}

	public class AHandler:IHandle<IMessage>
	{
		public void Handle(IMessage message)
		{
			
		}
	}
}
