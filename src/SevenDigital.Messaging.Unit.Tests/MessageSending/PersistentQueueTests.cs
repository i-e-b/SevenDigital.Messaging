using System;
using DiskQueue;
using NSubstitute;
using NUnit.Framework;
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
	}

	public class PersistentQueueTests
	{
		protected PersistentWorkQueue _subject;
		protected IPersistentQueue _queue;
		protected IPersistentQueueSession _session;
		protected IPersistentQueueFactory _queueFactory;
		protected byte[] _a_message;
		protected byte[] _b_message;

		[SetUp]
		public void setup()
		{
			_a_message = new byte[]{1,2,3,4};
			_b_message = new byte[] { 4, 3, 2, 1 };

			_session = Substitute.For<IPersistentQueueSession>();

			_queue = Substitute.For<IPersistentQueue>();
			_queue.OpenSession().Returns(_session);

			_queueFactory = Substitute.For<IPersistentQueueFactory>();
			_queueFactory.PrepareQueue().Returns(_queue);

			_subject = new PersistentWorkQueue(_queueFactory);
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