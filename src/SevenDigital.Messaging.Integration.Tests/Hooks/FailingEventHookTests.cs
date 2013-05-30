using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class FailingEventHookTests
	{
		IReceiver node_factory;
		private ISenderNode senderNode;

		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[TestFixtureSetUp]
		public void StartMessaging()
		{
			Helper.SetupTestMessaging();
		}

		[SetUp]
		public void SetUp()
		{
			node_factory = MessagingSystem.Receiver();
			senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void Should_trigger_all_event_hooks_with_message_when_sending_and_receiving_a_message()
		{
			MessagingSystem.Events
				.AddEventHook<FailingHook>()
				.AddEventHook<SucceedingHook>();

			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(SucceedingHook.SentEvent.WaitOne(ShortInterval), Is.True, "Hook one didn't get sent event");
				Assert.That(SucceedingHook.ReceivedEvent.WaitOne(ShortInterval), Is.True, "Hook one didn't get received event");
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}


	public class SucceedingHook : IEventHook
	{
		public static AutoResetEvent SentEvent = new AutoResetEvent(false);
		public static AutoResetEvent ReceivedEvent = new AutoResetEvent(false);

		public void MessageSent(IMessage msg)
		{
			SentEvent.Set();
		}

		public void MessageReceived(IMessage msg)
		{
			ReceivedEvent.Set();
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex) { }
	}

	public class FailingHook : IEventHook
	{
		public void MessageSent(IMessage msg)
		{
			Console.WriteLine("Failing on send");
			throw new Exception("I failed during sending");
		}

		public void MessageReceived(IMessage msg)
		{
			Console.WriteLine("Failing on receive");
			throw new Exception("I failed during receive");
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			Console.WriteLine("Failing on failed");
			throw new Exception("I failed during failed");
		}
	}
}
