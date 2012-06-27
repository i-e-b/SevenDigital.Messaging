using System;
using MassTransit;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Domain;
using SevenDigital.Messaging.Services;
using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Unit.Tests.Services.NodeTests
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
			_subject = new SenderNode(new Host("host"), new Endpoint("endpoint"), _serviceBusFactory.Object);
		}

		[Test]
		public void Should_only_create_service_bus_once_message_on_service_bus()
		{
			_subject.SendMessage(new Mock<IMessage>().Object);
			_subject.SendMessage(new Mock<IMessage>().Object);

			_serviceBusFactory.Verify( f => f.Create(It.IsAny<Uri>()), Times.Once());
		}

		[Test]
		public void Should_create_service_bus_with_address_property()
		{
			_subject.SendMessage(new Mock<IMessage>().Object);
			var address = new Uri("rabbitmq://host/endpoint");

			_serviceBusFactory.Verify(f => f.Create(address), Times.Once());
		}
	}
}