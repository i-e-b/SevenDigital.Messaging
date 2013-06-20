using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
	[TestFixture]
	public class LoopbackShutdownTests
	{
		
 		[Test]
		public void should_be_able_to_query_loopback_events_and_registered_handlers_after_shutdown ()
 		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Control.Shutdown();

			Assert.That(
				MessagingSystem.Testing.LoopbackHandlers().ForMessage<IMessage>(),
				Is.Not.Null);
 		}
		
 		[Test]
		public void should_be_able_to_query_message_traffic_after_shutdown ()
 		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Control.Shutdown();

			Assert.That(
				MessagingSystem.Testing.LoopbackEvents(),
				Is.Not.Null);
 		}

		[Test]
		public void configuring_messaging_after_shutting_down_loopback_mode_results_in_default_mode ()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Control.Shutdown();
			MessagingSystem.Configure.WithDefaults();

			Assert.That(
				MessagingSystem.Receiver(),
				Is.InstanceOf<Receiver>());
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}
	}
}