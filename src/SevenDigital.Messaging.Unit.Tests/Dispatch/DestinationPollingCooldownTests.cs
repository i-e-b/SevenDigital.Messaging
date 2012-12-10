using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class DestinationPollingCooldownTests
	{
		IDestinationPoller subject;
		IMessagingBase messagingBase;
		ISleepWrapper sleepWrapper;
		IMessageDispatcher dispatcher;
		IThreadPoolWrapper pool;

		[SetUp]
		public void A_destination_poller_with_a_dispatcher ()
		{
			messagingBase = Substitute.For<IMessagingBase>();
			sleepWrapper = Substitute.For<ISleepWrapper>();
			pool = Substitute.For<IThreadPoolWrapper>();

			dispatcher = Substitute.For<IMessageDispatcher>();
			dispatcher.HandlersInflight.Returns(2, 1, 0);

			subject = new DestinationPoller(messagingBase, sleepWrapper, dispatcher, pool);
		}

		[Test]
		public void When_stopping_Should_wait_until_handlers_inflight_returns_zero ()
		{
			subject.Stop();

			var calls = dispatcher.ReceivedCalls().Select(c=>c.GetMethodInfo()).ToList();

			Assert.That(calls.Count(c=>c.Name == "get_HandlersInflight"),
				Is.EqualTo(3));
		}

		[Test]
		public void Should_sleep_while_waiting ()
		{
			subject.Stop();

			sleepWrapper.Received(2).Sleep(100);
		}
	}
}
