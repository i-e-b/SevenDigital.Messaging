using System;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests._Helpers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Handlers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Api
{
	[TestFixture]
	public class PurgingQueues
	{
		IReceiver _receiver;
		private ISenderNode _senderNode;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(30); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[SetUp]
		public void SetUp()
		{
			// Start messaging, put everything in place
			Helper.SetupTestMessaging();
			_receiver = MessagingSystem.Receiver();
			_senderNode = MessagingSystem.Sender();

			using (_receiver.Listen(_=>_.Handle<IColourMessage>().With<ColourMessageHandler>())) { }
			_senderNode.SendMessage(new GreenMessage());
			MessagingSystem.Control.Shutdown(); // should cause message to flush

			// Start messaging again with purging:
			Helper.SetupTestMessaging();
		}

		[Test]
		public void Should_not_get_messages_waiting_on_queue_when_starting_a_new_listener()
		{
			ColourMessageHandler.AutoResetEvent.Reset();
			using (_receiver.Listen(_=>_.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
				Assert.That(colourSignal, Is.False);
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}
}
