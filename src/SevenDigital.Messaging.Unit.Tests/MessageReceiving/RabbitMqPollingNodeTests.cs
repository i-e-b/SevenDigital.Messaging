using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class RabbitMqPollingNodeTests
	{
		RabbitMqPollingNode _subject;
		IRoutingEndpoint _endpoint;
		IMessagingBase _messagingBase;
		ISleepWrapper _sleepWrapper;

		[SetUp]
		public void setup()
		{
			_endpoint = Substitute.For<IRoutingEndpoint>();
			_messagingBase = Substitute.For<IMessagingBase>();
			_sleepWrapper = Substitute.For<ISleepWrapper>();

			_endpoint.ToString().Returns("test");

			_subject = new RabbitMqPollingNode(_endpoint, _messagingBase, _sleepWrapper);
		}

		[Test]
		public void blocking_wait_should_return_immediately ()
		{
			Assert.That(_subject.BlockUntilReady(), Is.True);
		}

		[Test]
		public void with_a_bound_message_type_try_dequeue_should_read_from_messaging ()
		{
			_subject.AddMessageType(typeof(IMessage));
			_subject.TryDequeue();
			_messagingBase.Received().TryStartMessage<IMessage>("test");
		}

		[Test]
		public void with_no_bound_message_types_try_dequeue_should_do_nothing ()
		{
			_subject.TryDequeue();
			_messagingBase.DidNotReceive().TryStartMessage<IMessage>("test");
		}
	}
}