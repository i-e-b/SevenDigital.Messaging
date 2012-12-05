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
		string destinationName;

		[SetUp]
		public void A_destination_poller_with_a_destination_to_watch ()
		{
			messagingBase = new Mock<IMessagingBase>();
			subject = new DestinationPoller(messagingBase.Object);
			destinationName = "a-destination";
			subject.AddDestinationToWatch(destinationName);
			subject.Start();
		}

		[TearDown]
		public void stop_poller ()
		{
			subject.Stop();
		}

		[Test]
		public void Should_try_to_get_message_from_destination ()
		{
			messagingBase.Verify(m=>m.GetMessage<IMessage>(destinationName));
			subject.Stop();
		}

		[Test]
		public void Should_sleep_if_no_message_found ()
		{
		}

		[Test]
		public void Should_try_to_get_another_message_if_message_found ()
		{
		}

		[Test]
		public void Should_dispatch_message_handlers_if_message_found ()
		{
		}
	}
}
