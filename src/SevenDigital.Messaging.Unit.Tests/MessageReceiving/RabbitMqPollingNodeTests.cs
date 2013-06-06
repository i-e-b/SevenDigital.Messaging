using System;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
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
		string _destinationName = "test";

		[SetUp]
		public void setup()
		{
			_endpoint = Substitute.For<IRoutingEndpoint>();
			_messagingBase = Substitute.For<IMessagingBase>();
			_sleepWrapper = Substitute.For<ISleepWrapper>();

			_endpoint.ToString().Returns(_destinationName);

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
			_messagingBase.Received().TryStartMessage<IMessage>(_destinationName);
		}

		[Test]
		public void with_no_bound_message_types_try_dequeue_should_do_nothing ()
		{
			_subject.TryDequeue();
			_messagingBase.DidNotReceive().TryStartMessage<IMessage>(_destinationName);
		}

		[Test]
		public void cant_enqueue_to_a_rabbit_polling_node ()
		{
			var ex = Assert.Throws<InvalidOperationException>(() => _subject.Enqueue(Substitute.For<IPendingMessage<object>>()));

			Assert.That(ex.Message, Contains.Substring("This queue self populates and doesn't currently support direct injection"));
		}

		[Test]
		public void length_always_returns_zero ()
		{
			Assert.That(_subject.Length(), Is.EqualTo(0));
		}

		[Test]
		public void adding_a_message_type_tries_to_rebuild_queues_in_messaging_base ()
		{
			_subject.AddMessageType(typeof(IMessage));
			_messagingBase.Received().ResetCaches();
			_messagingBase.Received().CreateDestination(typeof(IMessage), _destinationName);
		}
		
		[Test]
		public void dequeuing_with_no_ready_messages_calls_sleeper ()
		{
			_subject.AddMessageType(typeof(IMessage));
			_messagingBase.TryStartMessage<IMessage>(_destinationName).Returns<IPendingMessage<IMessage>>(c => null);

			for (int i = 0; i < 5; i++)
			{
				_subject.TryDequeue();
			}
			_messagingBase.Received(5).TryStartMessage<IMessage>(_destinationName);
			_sleepWrapper.Received(5).SleepMore();
			_sleepWrapper.DidNotReceive().Reset();
		}

		[Test]
		public void when_a_message_becomes_available_the_sleeper_is_reset ()
		{
			_subject.AddMessageType(typeof(IMessage));
			_messagingBase.TryStartMessage<IMessage>(_destinationName).Returns(Substitute.For<IPendingMessage<IMessage>>());

			for (int i = 0; i < 5; i++)
			{
				_subject.TryDequeue();
			}
			_messagingBase.Received(5).TryStartMessage<IMessage>(_destinationName);
			_sleepWrapper.DidNotReceive().SleepMore();
			_sleepWrapper.Received(5).Reset();
		}

		[Test]
		public void unknown_queue_exceptions_are_caught_and_cause_queues_to_be_rebuilt ()
		{
			_subject.AddMessageType(typeof(IMessage));
			_messagingBase.TryStartMessage<IMessage>(_destinationName).Returns(c=> { throw UnknownQueueException(); });

			Assert.DoesNotThrow(() => _subject.TryDequeue());
			
			_messagingBase.Received().ResetCaches();
			_messagingBase.Received().CreateDestination(typeof(IMessage), _destinationName);
		}

		[Test]
		public void general_exceptions_from_messaging_base_are_passed_up ()
		{
			_subject.AddMessageType(typeof(IMessage));
			var ex = new Exception("Any kind of exception");
			_messagingBase.TryStartMessage<IMessage>(_destinationName).Returns(c=> { throw ex; });

			var exr = Assert.Throws<Exception>(() => _subject.TryDequeue());
			Assert.That(exr, Is.EqualTo(ex));
		}

		Exception UnknownQueueException()
		{
			return new OperationInterruptedException(
				new ShutdownEventArgs(ShutdownInitiator.Application, 404, null));
		}
	}
}