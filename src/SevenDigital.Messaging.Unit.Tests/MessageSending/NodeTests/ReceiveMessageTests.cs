using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class ReceiveMessageTests
	{
		ReceiverNode _receiverNode;
		Mock<IMessageDispatch> messageDispatch;
		IMessageBinding<IFakeMessage> _messageBinding;

		[SetUp]
		public void SetUp()
		{
			messageDispatch = new Mock<IMessageDispatch>();

			_receiverNode = new ReceiverNode(new Host("host"), new Endpoint("endpoint"), messageDispatch.Object);

			_messageBinding = _receiverNode.Handle<IFakeMessage>();
		}

		[Test]
		public void Should_connect_to_endpoint()
		{
			//_serviceBusFactory.Verify(sbf => sbf.Create());
		}

		[Test]
		public void Should_connect_to_endpoint_with_correct_uri()
		{
			//_serviceBusFactory.Verify(sbf => sbf.Create());
		}

		[Test]
		public void Should_call_message_binding_factory()
		{
			Assert.That(_messageBinding, Is.TypeOf<HandlerTriggering<IFakeMessage>>());
		}

	}
	public interface IFakeMessage : IMessage {}
}