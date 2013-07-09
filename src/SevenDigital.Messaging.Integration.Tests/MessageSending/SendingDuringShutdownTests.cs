using System;
using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class SendingDuringShutdownTests
	{
		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.FromSeconds(10));
			MessagingSystem.Control.Shutdown();
		}

		[Test]
		public void shutdown_waits_for_messages_to_send()
		{
			MessagingSystem.Configure.WithDefaults();
			ObjectFactory.Configure(map => map.For<ITestEvents>().Singleton().Use<TestEvents>());
			MessagingSystem.Events.AddEventHook<TestEventHook>();

			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.FromSeconds(10));

			var sender = MessagingSystem.Sender();

			for (int i = 0; i < 1000; i++)
			{
				sender.SendMessage(new GreenMessage());
			}
			MessagingSystem.Control.Shutdown();

			var sent = MessagingSystem.Testing.LoopbackEvents().SentMessages.Count();
			Console.WriteLine(sent);

			Assert.That(sent, Is.EqualTo(1000));
		}
		
		[Test]
		public void shutdown_timeout_is_respected()
		{
			MessagingSystem.Configure.WithDefaults();
			ObjectFactory.Configure(map => map.For<ITestEvents>().Singleton().Use<TestEvents>());
			MessagingSystem.Events.AddEventHook<TestEventHook>();

			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.Zero);

			var sender = MessagingSystem.Sender();

			for (int i = 0; i < 1000; i++)
			{
				sender.SendMessage(new GreenMessage());
			}
			MessagingSystem.Control.Shutdown();

			var sent = MessagingSystem.Testing.LoopbackEvents().SentMessages.Count();
			Console.WriteLine(sent);

			Assert.That(sent, Is.LessThan(1000));
		}


		public class DummyHandler:IHandle<IColourMessage>
		{
			public void Handle(IColourMessage message) { }
		}
	}
}