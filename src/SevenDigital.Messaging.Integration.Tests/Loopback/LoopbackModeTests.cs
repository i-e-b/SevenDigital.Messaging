using System;
using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests.Loopback
{
	[TestFixture]
	public class LoopbackModeTests
	{
		[Test]
		public void Loopback_mode_configures_correctly ()
		{
			MessagingSystem.Control.Shutdown();
			MessagingSystem.Configure.WithLoopbackMode();
			
			using (var listener = MessagingSystem.Receiver().Listen())
			{
				IntegrationHandler.Sent = false;
				listener.Handle<IMessage>().With<IntegrationHandler>();
				MessagingSystem.Sender().SendMessage(new TestMessage());
				Assert.That(IntegrationHandler.Sent, Is.True);
			}
			
			MessagingSystem.Control.Shutdown();
		}
	}

	public class TestMessage:IMessage
	{
		public Guid CorrelationId { get; set; }
	}

	public class IntegrationHandler:IHandle<IMessage>
	{
		public static bool Sent = false;

		public void Handle(IMessage message)
		{
			Sent = true;
		}
	}
}
