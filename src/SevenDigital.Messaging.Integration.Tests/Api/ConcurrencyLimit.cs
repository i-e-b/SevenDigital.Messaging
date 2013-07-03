using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class ConcurrencyLimit
	{
		[Test]
		public void can_set_concurreny_limit_in_default_mode ()
		{
			MessagingSystem.Configure.WithDefaults();
			MessagingSystem.Control.SetConcurrentHandlers(1);
			MessagingSystem.Control.Shutdown();

			Assert.Pass();
		}

		[Test]
		public void concurrency_limit_is_obeyed_in_default_mode ()
		{
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();
			MessagingSystem.Control.SetConcurrentHandlers(1);

			CountingHandler.MaxCount = 0;
			CountingHandler.Count = 0;
			MessagingSystem.Receiver().Listen().Register(
				with => with.Handle<IMessage>().With<CountingHandler>());

			for (int i = 0; i < 20; i++)
			{
				MessagingSystem.Sender().SendMessage(new GreenMessage());
			}

			Thread.Sleep(1500);
			MessagingSystem.Control.Shutdown();
			Assert.That(CountingHandler.MaxCount, Is.EqualTo(1));
		}

		public class CountingHandler:IHandle<IMessage>
		{
			public static volatile int Count, MaxCount;

			public void Handle(IMessage message)
			{
				Interlocked.Increment(ref Count);
				Thread.Sleep(200);
				if (Count > MaxCount) MaxCount = Count;
				Interlocked.Decrement(ref Count);
			}
		}

		[Test]
		public void can_set_concurreny_limit_in_loopback_mode ()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Control.SetConcurrentHandlers(1);
			MessagingSystem.Control.Shutdown();

			Assert.Pass();
		}
	}
}