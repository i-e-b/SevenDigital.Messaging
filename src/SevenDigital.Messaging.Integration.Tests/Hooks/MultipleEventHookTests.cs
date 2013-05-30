using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class MultipleEventHookTests
	{
		IReceiver node_factory;
		private ISenderNode senderNode;

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
			node_factory = MessagingSystem.Receiver();
			senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void Should_trigger_all_event_hooks_with_message_when_sending_and_receiving_a_message()
		{
			MessagingSystem.Events
				.AddEventHook<WaitingHookOne>()
				.AddEventHook<WaitingHookTwo>();

			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(WaitingHookOne.SentEvent.WaitOne(LongInterval), Is.True, "Hook one didn't get sent event");
				Assert.That(WaitingHookOne.ReceivedEvent.WaitOne(LongInterval), Is.True, "Hook one didn't get received event");
				Assert.That(WaitingHookTwo.SentEvent.WaitOne(LongInterval), Is.True, "Hook one didn't get sent event");
				Assert.That(WaitingHookTwo.ReceivedEvent.WaitOne(LongInterval), Is.True, "Hook one didn't get received event");
			}
		}

		[Test]
		public void Should_not_trigger_any_event_hooks_when_hooks_have_been_cleared()
		{
			MessagingSystem.Events.ClearEventHooks();

			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(WaitingHookOne.SentEvent.WaitOne(ShortInterval), Is.False, "Hook one didn't get sent event");
				Assert.That(WaitingHookOne.ReceivedEvent.WaitOne(ShortInterval), Is.False, "Hook one didn't get received event");
				Assert.That(WaitingHookTwo.SentEvent.WaitOne(ShortInterval), Is.False, "Hook one didn't get sent event");
				Assert.That(WaitingHookTwo.ReceivedEvent.WaitOne(ShortInterval), Is.False, "Hook one didn't get received event");
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }

		public class WaitingHookOne : IEventHook
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
		public class WaitingHookTwo : IEventHook
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
	}
}
