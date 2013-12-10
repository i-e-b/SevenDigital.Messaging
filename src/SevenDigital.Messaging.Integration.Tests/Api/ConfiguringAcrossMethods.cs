using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Api
{
	[TestFixture]
	public class ConfiguringAcrossMethods
	{
		[SetUp]
		public void setup()
		{
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();

			ConfigureHandlersInAnotherMethod();
			GC.Collect();
		}

		[Test]
		public void everything_is_configured_correctly ()
		{
			MessagingSystem.Sender().SendMessage(new GreenMessage());

			while (CountingHandler.Count < 1)
			{
				Thread.Sleep(100);
			}
			Assert.Pass();
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}

		void ConfigureHandlersInAnotherMethod()
		{
			MessagingSystem.Receiver().Listen(_=>_
				.Handle<IMessage>().With<CountingHandler>());
		}

#pragma warning disable 420
		public class CountingHandler:IHandle<IMessage>
		{
			public static volatile int Count;
			public void Handle(IMessage message)
			{
				Interlocked.Increment(ref Count);
			}
		}
#pragma warning restore 420
	}
}