using System;
using MassTransit;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class SendMessageTests
	{
		Mock<IServiceBusFactory> _serviceBusFactory;
		Mock<IServiceBus> _serviceBus;
		SenderNode _subject;

		[SetUp]
		public void SetUp()
		{
			_serviceBus = new Mock<IServiceBus>();
			_serviceBusFactory = new Mock<IServiceBusFactory>();
			_serviceBusFactory.Setup(f => f.Create(It.IsAny<Uri>())).Returns(_serviceBus.Object);
		    var endpointGenerator = new Mock<ISenderEndpointGenerator>();
		    endpointGenerator.Setup(g => g.Generate()).Returns(new Endpoint("endpoint"));
		    _subject = new SenderNode(new Host("host"), endpointGenerator.Object, _serviceBusFactory.Object);
		}

		[Test]
		public void Should_only_create_service_bus_once_message_on_service_bus()
		{
			_subject.SendMessage(new Mock<IMessage>().Object);
			_subject.SendMessage(new Mock<IMessage>().Object);

			_serviceBusFactory.Verify( f => f.Create(It.IsAny<Uri>()), Times.Once());
		}

		[Test]
		public void Should_call_send_message_on_service_bus ()
		{
			var msg = new DummyMessage();
			_subject.SendMessage(msg);

			// This form of publish is an instance method and can be mocked. The others are extensions and can't be mocked
			_serviceBus.Verify(b=>b.Publish(msg, AnyContextCallback()));
		}

		static Action<IPublishContext<DummyMessage>> AnyContextCallback()
		{
			return It.IsAny<Action<IPublishContext<DummyMessage>>>();
		}

		[Test]
		public void Should_create_service_bus_with_address_property()
		{
			_subject.SendMessage(new Mock<IMessage>().Object);
			var address = new Uri("rabbitmq://host/endpoint");

			_serviceBusFactory.Verify(f => f.Create(address), Times.Once());
		}

		class DummyMessage : IMessage
		{
			public Guid CorrelationId
			{
				get { return Guid.Empty; }
				set { }
			}
		}
	}
}