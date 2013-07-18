using System;
using System.Diagnostics;
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
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();
			ObjectFactory.Configure(map => map.For<ITestEvents>().Singleton().Use<TestEvents>());
			MessagingSystem.Events.AddEventHook<TestEventHook>();

			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.FromSeconds(10));

			var sender = MessagingSystem.Sender();

			for (int i = 0; i < 100; i++)
			{
				sender.SendMessage(new GreenMessage());
			}
			MessagingSystem.Control.Shutdown();

			var sent = MessagingSystem.Testing.LoopbackEvents().SentMessages.Count();
			Console.WriteLine(sent);

			Assert.That(sent, Is.EqualTo(100));
		}
		
		[Test]
		public void shutdown_timeout_is_respected()
		{
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();
			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.Zero);

			var sender = MessagingSystem.Sender();

			for (int i = 0; i < 100; i++)
			{
				sender.SendMessage(new GreenMessage());
			}
	
			var sw = new Stopwatch();
			sw.Start();
			MessagingSystem.Control.Shutdown();
			sw.Stop();

			Assert.That(sw.ElapsedMilliseconds, Is.LessThan(500));
		}


		public class DummyHandler:IHandle<IColourMessage>
		{
			public void Handle(IColourMessage message) { }
		}
	}
}