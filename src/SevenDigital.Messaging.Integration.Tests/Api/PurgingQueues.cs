using System;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class PurgingQueues
	{
		IReceiver _receiver;
		private ISenderNode _senderNode;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(30); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[TestFixtureSetUp]
		public void StartMessaging()
		{
			Helper.SetupTestMessaging();
		}

		[SetUp]
		public void SetUp()
		{
			_receiver = MessagingSystem.Receiver();
			_senderNode = MessagingSystem.Sender();

			using (var l = _receiver.Listen())
			{
				l.Handle<IColourMessage>().With<ColourMessageHandler>();
			}
			_senderNode.SendMessage(new GreenMessage());
		}

		[Test]
		public void Should_not_get_messages_waiting_on_queue_when_starting_a_new_listener()
		{
			ColourMessageHandler.AutoResetEvent.Reset();
			using (var receiverNode = _receiver.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
				Assert.That(colourSignal, Is.False);
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}
}
