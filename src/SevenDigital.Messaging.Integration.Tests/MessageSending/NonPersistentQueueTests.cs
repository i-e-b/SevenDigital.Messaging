using System.Text;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class NonPersistentQueueTests
	{
		InMemoryQueueBridge _subject;
		int _received;
		int _sent;

		[SetUp]
		public void setup()
		{
			_received = 0;
			_sent = 0;
			_subject = new InMemoryQueueBridge();
		}

		[Test]
		public void the_non_persistent_queue_is_thread_safe ()
		{
			var t0 = new Thread(Spin);
			var t1 = new Thread(Spin);
			var t2 = new Thread(Spin);

			t0.Start();
			t1.Start();
			t2.Start();

			t0.Join();
			t1.Join();
			t2.Join();

			Assert.That(_sent, Is.EqualTo(3000));
			Assert.That(_received, Is.EqualTo(3000));
		}

		void Spin()
		{
			for (int i = 0; i < 1000; i++)
			{
				using (var q = _subject.OpenSession())
				{
					q.Enqueue(Encoding.ASCII.GetBytes("Spinning"));
					Interlocked.Increment(ref _sent);
				}
				
				Thread.Sleep(1);
				
				using (var q = _subject.OpenSession())
				{
					var s = Encoding.ASCII.GetString(q.Dequeue());
					if (s == "Spinning") Interlocked.Increment(ref _received);
				}
			}
		}
	}
}