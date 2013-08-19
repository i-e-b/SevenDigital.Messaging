using System;
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

			Assert.That(MessagingSystem.Testing.ConcurrencyLimit(), Is.EqualTo(Environment.ProcessorCount));
			MessagingSystem.Control.SetConcurrentHandlers(1);
			Assert.That(MessagingSystem.Testing.ConcurrencyLimit(), Is.EqualTo(1));
			
			
			MessagingSystem.Control.Shutdown();

			Assert.Pass();
		}

		[Test]
		public void concurrency_can_be_set_beyond_current_number_of_cores ()
		{
			MessagingSystem.Control.SetConcurrentHandlers(10);
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();

			CountingHandler.Reset();
			MessagingSystem.Receiver().Listen(_=>_
				.Handle<IMessage>().With<CountingHandler>()
			);

			for (int i = 0; i < 200; i++)
			{
				MessagingSystem.Sender().SendMessage(new GreenMessage());
			}
			while (CountingHandler.TotalCount < 10)
			{
				Thread.Sleep(250);
			}
			MessagingSystem.Control.Shutdown();
			Assert.That(CountingHandler.MaxCount, Is.EqualTo(10));
		}
		

		[Test]
		public void concurrency_limit_is_obeyed_in_default_mode ()
		{
			MessagingSystem.Control.SetConcurrentHandlers(1);
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();

			CountingHandler.Reset();
			MessagingSystem.Receiver().Listen(_=>_
				.Handle<IMessage>().With<CountingHandler>()
			);

			for (int i = 0; i < 20; i++)
			{
				MessagingSystem.Sender().SendMessage(new GreenMessage());
			}
			while (CountingHandler.TotalCount < 10)
			{
				Thread.Sleep(250);
			}
			MessagingSystem.Control.Shutdown();
			Assert.That(CountingHandler.MaxCount, Is.EqualTo(1));
		}

		public class CountingHandler:IHandle<IMessage>
		{
			public static volatile int Count, MaxCount, TotalCount;

			public void Handle(IMessage message)
			{
#pragma warning disable 420
				Interlocked.Increment(ref TotalCount);
				Interlocked.Increment(ref Count);
				Thread.Sleep(200);
				if (Count > MaxCount) MaxCount = Count;
				Interlocked.Decrement(ref Count);
#pragma warning restore 420
			}

			public static void Reset()
			{
				Count = MaxCount = TotalCount = 0;
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