using System;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class SendMessageTests
	{
		Mock<IMessageDispatch> messageDispatch;
		SenderNode _subject;

		[SetUp]
		public void SetUp()
		{
			messageDispatch = new Mock<IMessageDispatch>();
		    var endpointGenerator = new Mock<ISenderEndpointGenerator>();
		    endpointGenerator.Setup(g => g.Generate()).Returns(new Endpoint("endpoint"));
		    _subject = new SenderNode(new Host("host"), endpointGenerator.Object, messageDispatch.Object);
		}

		[Test]
		public void Should_only_create_service_bus_once_message_on_service_bus()
		{
			_subject.SendMessage(new Mock<IMessage>().Object);
			_subject.SendMessage(new Mock<IMessage>().Object);
		}

		[Test]
		public void Should_call_send_message_on_service_bus ()
		{
			var msg = new DummyMessage();
			_subject.SendMessage(msg);

			// This form of publish is an instance method and can be mocked. The others are extensions and can't be mocked
			messageDispatch.Verify(b=>b.Publish(msg));
		}

		[Test]
		public void Should_send_message_through_message_dispatch()
		{
			var msg = new Mock<IMessage>().Object;
			_subject.SendMessage(msg);

			messageDispatch.Verify(m=>m.Publish(msg));
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