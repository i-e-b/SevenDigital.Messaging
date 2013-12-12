using System;
using System.IO;
using System.Text;
using System.Threading;
using DiskQueue;
using NUnit.Framework;
using SevenDigital.Messaging.ConfigurationActions;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.LocalQueue
{
	[TestFixture]
	public class SendToTests
	{
		LocalQueueConfig _conf;

		const string ReadQueuePath = "./readQueue";
		const string WriteQueuePath = "./writeQueue";
		
		[SetUp]
		public void setup()
		{
			DeleteOldQueues();

			MessagingSystem
				.Configure
				.WithLocalQueue(ReadQueuePath)
				.SendTo(WriteQueuePath);

			MessagingSystem.Receiver().Listen(_=>_.Handle<IColourMessage>().With<PassAlongHandler>());
			_conf = ObjectFactory.GetInstance<LocalQueueConfig>();
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}

		[Test]
		public void can_read_from_one_queue_and_send_to_another ()
		{
			Assert.That(_conf.IncomingPath, Is.Not.EqualTo(_conf.WritePath), "Path config is not correct");

			var msgString = "{\"__type\":\"SevenDigital.Messaging.Integration.Tests._Helpers.Messages.IColourMessage, SevenDigital.Messaging.Integration.Tests\",\"CorrelationId\":\"ede08be9f4a44d148d81317a9288bbfb\",\"Text\":\"Red\",\"__contracts\":\"SevenDigital.Messaging.Integration.Tests._Helpers.Messages.IColourMessage, SevenDigital.Messaging.Integration.Tests;SevenDigital.Messaging.IMessage, SevenDigital.Messaging\"}";
			var data = Encoding.UTF8.GetBytes(msgString);

			// Drop a fake message on the read queue:
			using (var q = PersistentQueue.WaitFor(_conf.IncomingPath, TimeSpan.FromSeconds(10)))
			using (var s = q.OpenSession())
			{
				s.Enqueue(data);
				s.Flush();
			}

			Assert.True(PassAlongHandler.Trigger.WaitOne(TimeSpan.FromSeconds(10)), "Message was not received");
			byte[] result;

			// Pick up message from target incoming queue:
			using (var q = PersistentQueue.WaitFor(_conf.WritePath, TimeSpan.FromSeconds(10)))
			using (var s = q.OpenSession())
			{
				result = s.Dequeue();
				s.Flush();
			}

			Assert.That(result, Is.Not.Null, "Message was not sent to the target queue");
			var resultString = Encoding.UTF8.GetString(result);
			Assert.That(resultString, Is.EqualTo(msgString), "Message was not transferred correctly");
		}

		public class PassAlongHandler:IHandle<IColourMessage>
		{
			public static AutoResetEvent Trigger = new AutoResetEvent(false);
			public void Handle(IColourMessage message)
			{
				MessagingSystem.Sender().SendMessage(message);
				Trigger.Set();
			}
		}

		static void DeleteOldQueues()
		{
			if (Directory.Exists(ReadQueuePath))
				Directory.Delete(ReadQueuePath, true);

			if (Directory.Exists(WriteQueuePath))
				Directory.Delete(WriteQueuePath, true);
		}
	}
}