﻿using System.Threading;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class DestinationMessagePollingTests
	{
		IDestinationPoller subject;
		Mock<IMessagingBase> messagingBase;
		Mock<ISleepWrapper> sleeper;
		Mock<IMessageDispatcher> dispatcher;
		Mock<IThreadPoolWrapper> pool;
		string destinationName;

		[SetUp]
		public void A_destination_poller_with_a_destination_to_watch ()
		{
			messagingBase = new Mock<IMessagingBase>();
			sleeper = new Mock<ISleepWrapper>();
			dispatcher = new Mock<IMessageDispatcher>();
			pool = new Mock<IThreadPoolWrapper>();
			destinationName = "a-destination";
			pool.Setup(m=>m.IsThreadAvailable()).Returns(true);

			subject = new DestinationPoller(messagingBase.Object, sleeper.Object, dispatcher.Object, pool.Object);
			subject.AddDestinationToWatch(destinationName);
		}

		[TearDown]
		public void stop_poller ()
		{
			subject.Stop();
		}

		[Test]
		public void Should_try_to_get_message_from_destination ()
		{
			subject.Start();
			Thread.Sleep(100);
			subject.Stop();
			messagingBase.Verify(m=>m.GetMessage<IMessage>(destinationName));
		}

		[Test]
		public void Can_start_poller_twice_without_exceptions ()
		{
			subject.Start();
			subject.Start();
			Assert.Pass();
		}

		[Test]
		public void Should_sleep_if_no_message_found ()
		{
			messagingBase.Setup(m=>m.GetMessage<IMessage>(destinationName))
				.Returns<IMessage>(null);
			subject.Start();
			Thread.Sleep(100);
			subject.Stop();
			sleeper.Verify(m=>m.Sleep());
		}

		[Test]
		public void Should_try_to_get_another_message_without_sleeping_if_message_found ()
		{
			messagingBase.Setup(m=>m.GetMessage<IMessage>(destinationName))
				.Returns(new Mock<IMessage>().Object);
			
			subject.Start();
			Thread.Sleep(100);
			subject.Stop();
			sleeper.Verify(m=>m.Sleep(), Times.Never());
		}

		[Test]
		public void Should_dispatch_message_handlers_if_message_found ()
		{
			var fakeMsg = new Mock<IMessage>().Object;
			messagingBase.Setup(m=>m.GetMessage<IMessage>(destinationName))
				.Returns(fakeMsg);
			
			subject.Start();
			Thread.Sleep(100);
			subject.Stop();
			dispatcher.Verify(m=>m.TryDispatch(fakeMsg));
		}

		[Test]
		public void If_thread_pool_is_full_should_not_try_to_get_messages ()
		{
			pool.Setup(m=>m.IsThreadAvailable()).Returns(false);
			subject.Start();
			Thread.Sleep(750);
			subject.Stop();
			messagingBase.Verify(m=>m.GetMessage<IMessage>(destinationName), Times.Never());
			sleeper.Verify(m=>m.Sleep());
		}

		[Test]
		public void should_continue_to_poll_until_stopped ()
		{
			int count = 0;
			messagingBase.Setup(m=>m.GetMessage<IMessage>(destinationName)).Callback(() => {
				count++;
			});

			subject.Start();
			Thread.Sleep(750);
			Assert.That(count, Is.GreaterThan(1));

		}

		[Test]
		public void Can_restart_after_stopping  ()
		{
			int[] count = {0};
			messagingBase.Setup(m=>m.GetMessage<IMessage>(destinationName)).Callback(() => {
				count[0]++;
			});

			subject.Start();
			while (count[0] < 1) { Thread.Sleep(1); }
			subject.Stop();
			count[0] = 0;
			subject.Start();
			Thread.Sleep(750);
			subject.Stop();
			Assert.That(count[0], Is.GreaterThan(1));

		}
	}
}
