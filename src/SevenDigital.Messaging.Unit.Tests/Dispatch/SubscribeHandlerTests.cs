using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Unit.Tests.LoopbackMessaging;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class SubscribeHandlerTests
	{
		INode subject;
		Mock<IMessagingBase> messagingBase;
		Mock<IDispatchController> dispatchController;
		Mock<IDestinationPoller> destinationPoller;
		const string destinationName = "woop";

		[SetUp]
		public void Subscribing_a_handler_with_a_message_dispatcher ()
		{
			messagingBase = new Mock<IMessagingBase>();
			destinationPoller = new Mock<IDestinationPoller>();
			dispatchController = new Mock<IDispatchController>();

			dispatchController.Setup(m=>m.CreatePoller(destinationName)).Returns(destinationPoller.Object);

			subject = new Node(messagingBase.Object, dispatchController.Object);
			subject.SetEndpoint(new Endpoint(destinationName));

			subject.SubscribeHandler<IDummyMessage, DummyHandler>();
		}

		[Test]
		public void Should_ensure_destination_is_set_up_for_node ()
		{
			messagingBase.Verify(m=>m.CreateDestination<IDummyMessage>(destinationName));
		}

		[Test]
		public void Should_add_type_and_action_to_poller ()
		{
			destinationPoller.Verify(m=>m.AddHandler<IDummyMessage, DummyHandler>());
		}

		[Test]
		public void Should_ensure_destination_poller_is_started ()
		{
			destinationPoller.Verify(m=>m.Start());
		}

		[Test]
		public void Should_make_sure_destination_poller_is_watching_target_destination ()
		{
			dispatchController.Verify(m=>m.CreatePoller(destinationName));
		}
	}
}
