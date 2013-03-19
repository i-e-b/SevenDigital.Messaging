using System;// ReSharper disable InconsistentNaming
using System.IO;
using System.Text;
using DiskQueue;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class StoreAndForwardSpike
	{
		INodeFactory _nodeFactory;
		private ISenderNode _senderNode;
		IMessageSerialiser _messageSerialisation;
		string _storagePath = @"/tmp/queue";

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(20); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[SetUp]
		public void SetUp()
		{
			Helper.SetupTestMessaging();
			ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
			_nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
			_senderNode = ObjectFactory.GetInstance<ISenderNode>();
			_messageSerialisation = ObjectFactory.GetInstance<IMessageSerialiser>();
			if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
		}

		[Test]
		public void should_have_a_singleton_persistent_queue_configured()
		{
			new MessagingConfiguration().WithDefaults();

			var queue_a = ObjectFactory.GetInstance<IPersistentQueue>();
			var queue_b = ObjectFactory.GetInstance<IPersistentQueue>();

			Assert.That(queue_a, Is.InstanceOf<PersistentQueue>());
			Assert.That(queue_a, Is.EqualTo(queue_b));
		}

		[Test, Description("the spike")]
		public void trying_to_stash_messages_to_disk_and_send_from_there()
		{
			var sampleMessage = new GreenMessage { CorrelationId = Guid.NewGuid() };
			var raw = _messageSerialisation.Serialise(sampleMessage);
			var bytes = Encoding.UTF8.GetBytes(raw);

			using (var q = new PersistentQueue(_storagePath))
			{
				using (var s = q.OpenSession())
				{
					s.Enqueue(bytes);
					s.Flush();
				}
				Assert.That(q.EstimatedCountOfItemsInQueue, Is.EqualTo(1));
			}

			using (var q2 = new PersistentQueue(_storagePath))
			{
				using (var s2 = q2.OpenSession())
				{
					var newBytes = s2.Dequeue();
					s2.Flush();
					var newRaw = Encoding.UTF8.GetString(newBytes);

					var returnedMessage = _messageSerialisation.DeserialiseByStack(newRaw);
					Assert.That(returnedMessage as IColourMessage, Is.Not.Null);
					var final = (IColourMessage)returnedMessage;

					Assert.That(final.CorrelationId, Is.EqualTo(sampleMessage.CorrelationId));
					Assert.That(q2.EstimatedCountOfItemsInQueue, Is.EqualTo(0));
				}
			}
		}

		[Test]
		public void can_have_two_sessions_open_at_once ()
		{
			var bytes = Encoding.UTF8.GetBytes("Hello");
			new MessagingConfiguration().WithDefaults();
			var queue = ObjectFactory.GetInstance<IPersistentQueue>();

			using (var s1 = queue.OpenSession())
			{
				using (var s2 = queue.OpenSession())
				{
					s2.Enqueue(bytes);
					s2.Flush();
					var message = Encoding.UTF8.GetString(s1.Dequeue());

					Assert.That(message, Is.EqualTo("Hello"));
				}
			}
		}

		[TestFixtureTearDown]
		public void Stop()
		{
			new MessagingConfiguration().Shutdown();
			if (Directory.Exists(_storagePath))
				Directory.Delete(_storagePath, true);
		}

	}
}