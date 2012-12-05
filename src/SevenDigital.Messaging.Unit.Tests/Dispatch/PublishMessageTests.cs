using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class PublishMessageTests
	{
		MessageDispatch subject;
		Mock<IMessagingBase> msg;

		[SetUp]
		public void A_message_dispatch ()
		{
			msg = new Mock<IMessagingBase>();
			subject = new MessageDispatch(msg.Object);
		}

		[Test]
		public void should_call_message_base_send_method ()
		{
			var messageObject = new Mock<IMessage>().Object;
			subject.Publish(messageObject);

			msg.Verify(m=>m.SendMessage(messageObject));
		}
	}
}
