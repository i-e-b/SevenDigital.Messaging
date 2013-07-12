using System;
using DiskQueue;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending
{
	[TestFixture]
	public class when_adding_an_item_to_the_persistent_queue : PersistentQueueTests
	{
		[Test]
		public void should_open_and_close_a_session()
		{
			_subject.Enqueue(_a_message);

			_queue.Received().OpenSession();
			_session.Received().Dispose();
		}

		[Test]
		public void should_write_item_to_queue()
		{
			_subject.Enqueue(_a_message);

			_session.Received().Enqueue(Arg.Any<byte[]>());
		}

		[Test]
		public void should_flush_queue_after_writing_and_before_closing()
		{
			_subject.Enqueue(_a_message);

			_session.Received().Flush();
		}
	}

	[TestFixture]
	public class when_dequeueing : PersistentQueueTests
	{

		[Test]
		public void dequeing_an_item_reads_and_closes_session_without_flushing ()
		{
			_subject.Enqueue(_a_message);
			_session.ClearReceivedCalls();

			_subject.TryDequeue();

			_session.Received().Dequeue();
			_session.Received().Dispose();

			_session.DidNotReceive().Flush();
		}
		[Test]
		public void dequeueing_a_second_item_before_the_first_is_ended_doesnt_use_the_session ()
		{
			_subject.Enqueue(_a_message);
			_subject.Enqueue(_b_message);
			_subject.TryDequeue();
			_session.ClearReceivedCalls();
			_queue.ClearReceivedCalls();

			_subject.TryDequeue();

			_queue.DidNotReceive().OpenSession();
		}
		[Test]
		public void dequeueing_a_second_item_before_the_first_is_ended_returns_empty ()
		{
			_subject.Enqueue(_a_message);
			_subject.Enqueue(_b_message);
			_session.ClearReceivedCalls();

			_subject.TryDequeue();
			var result = _subject.TryDequeue();

			Assert.That(result.HasItem, Is.False);
		}
		/*
		THESE NEED TO GO TO INTEGRATION TESTS
		[Test]
		public void ending_an_item_by_finishing_allows_the_next_item_to_be_dequeued ()
		{
			_subject.Enqueue(_a_message);
			_subject.Enqueue(_b_message);

			var expected_A = _subject.TryDequeue();
			expected_A.Finish();
			var expected_B = _subject.TryDequeue();
			
			Assert.That(expected_A.HasItem, Is.True);
			Assert.That(expected_B.HasItem, Is.True);
		}
		[Test]
		public void ending_an_item_by_cancelling_means_the_cancelled_item_will_be_the_next_to_dequeue ()
		{
			_subject.Enqueue(_a_message);
			_subject.Enqueue(_b_message);

			var expected_A = _subject.TryDequeue();
			expected_A.Cancel();
			var expected_B = _subject.TryDequeue();
			
			Assert.That(expected_A.HasItem, Is.True);
			Assert.That(expected_B.HasItem, Is.True);
		}
		*/
	}

	public class PersistentQueueTests
	{
		protected IMessageSerialiser _serialiser;
		protected PersistentWorkQueue _subject;
		protected IPersistentQueue _queue;
		protected IPersistentQueueSession _session;
		protected IPersistentQueueFactory _queueFactory;
		protected IMessage _a_message;
		protected IMessage _b_message;

		[SetUp]
		public void setup()
		{
			_a_message = new MessageWithUniqueId();
			_b_message = new MessageWithUniqueId();
			_serialiser = Substitute.For<IMessageSerialiser>();

			_session = Substitute.For<IPersistentQueueSession>();

			_queue = Substitute.For<IPersistentQueue>();
			_queue.OpenSession().Returns(_session);

			_queueFactory = Substitute.For<IPersistentQueueFactory>();
			_queueFactory.PrepareQueue(Arg.Any<string>()).Returns(_queue);

			_subject = new PersistentWorkQueue(_serialiser, _queueFactory);
		}
	}

	public class MessageWithUniqueId : IMessage
	{
		public MessageWithUniqueId()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
	}
}