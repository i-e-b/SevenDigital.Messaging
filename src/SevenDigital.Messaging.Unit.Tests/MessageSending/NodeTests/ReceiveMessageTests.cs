using System;
using MassTransit;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class ReceiveMessageTests
	{
		Mock<IServiceBusFactory> _serviceBusFactory;
		ReceiverNode _receiverNode;
		Mock<IServiceBus> _serviceBus;
		IMessageBinding<IFakeMessage> _messageBinding;

		[SetUp]
		public void SetUp()
		{
			_serviceBusFactory = new Mock<IServiceBusFactory>();
			_serviceBus = new Mock<IServiceBus>();
			_serviceBusFactory.Setup(sbf => sbf.Create(It.IsAny<Uri>())).Returns(_serviceBus.Object);

			_receiverNode = new ReceiverNode(new Host("host"), new Endpoint("endpoint"), _serviceBusFactory.Object);

			_messageBinding = _receiverNode.Handle<IFakeMessage>();
		}

		[Test]
		public void Should_connect_to_endpoint()
		{
			_serviceBusFactory.Verify(sbf => sbf.Create(It.IsAny<Uri>()));
		}

		[Test]
		public void Should_connect_to_endpoint_with_correct_uri()
		{
			_serviceBusFactory.Verify(sbf => sbf.Create(new Uri("rabbitmq://host/endpoint")));
		}

		[Test]
		public void Should_call_message_binding_factory()
		{
			Assert.That(_messageBinding, Is.TypeOf<MessageBinding<IFakeMessage>>());
		}

	}
	public interface IFakeMessage : IMessage {}
}