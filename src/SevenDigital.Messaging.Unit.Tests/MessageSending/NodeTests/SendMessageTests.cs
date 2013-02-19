using System;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class SendMessageTests
	{
		Mock<IMessagingBase> messageDispatch;
		SenderNode _subject;

		[SetUp]
		public void SetUp()
		{
			messageDispatch = new Mock<IMessagingBase>();
		    var endpointGenerator = new Mock<ISenderEndpointGenerator>();
		    endpointGenerator.Setup(g => g.Generate()).Returns(new Endpoint("endpoint"));
		    _subject = new SenderNode(messageDispatch.Object, null);
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

			messageDispatch.Verify(b=>b.SendMessage(msg));
		}

		[Test]
		public void Should_send_message_through_message_dispatch()
		{
			var msg = new Mock<IMessage>().Object;
			_subject.SendMessage(msg);

			messageDispatch.Verify(m=>m.SendMessage(msg));
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