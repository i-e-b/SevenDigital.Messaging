using System;// ReSharper disable InconsistentNaming
using System.Text;
using DiskQueue;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
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
			
		}

		[Test]
		public void trying_to_stash_messages_to_disk_and_send_from_there ()
		{
			var sampleMessage = new GreenMessage{CorrelationId = Guid.NewGuid()};
			var raw = _messageSerialisation.Serialise(sampleMessage);
			var bytes = Encoding.UTF8.GetBytes(raw);

			using (var q = new PersistentQueue(@"C:\temp\queue"))
			{
				using (var s = q.OpenSession())
				{
					s.Enqueue(bytes);
					s.Flush();
				}
			}

			using (var q2 = new PersistentQueue(@"C:\temp\queue"))
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
				}
			}
		}

		[TestFixtureTearDown]
		public void Stop() { new MessagingConfiguration().Shutdown(); }

	}
}