using System;// ReSharper disable InconsistentNaming
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class SendingAndReceivingTests
	{
		IReceiver _receiver;
		private ISenderNode _sender;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(20); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[SetUp]
		public void SetUp()
		{
			Helper.SetupTestMessaging();

			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_receiver = MessagingSystem.Receiver();
			_sender = MessagingSystem.Sender();
		}

		[Test]
		public void Handler_should_react_for_all_message_types_it_is_handling()
		{
			using (var receiverNode = _receiver.Listen())
			{
				AllColourMessagesHandler.Prepare();

				receiverNode.Handle<IColourMessage>().With<AllColourMessagesHandler>();
				receiverNode.Handle<ITwoColoursMessage>().With<AllColourMessagesHandler>();

				_sender.SendMessage(new RedMessage());
				var signal1 = AllColourMessagesHandler.AutoResetEventForColourMessage.WaitOne(LongInterval);
				_sender.SendMessage(new GreenWhiteMessage());

				var signal2 = AllColourMessagesHandler.AutoResetEventForTwoColourMessage.WaitOne(LongInterval);
				Assert.That(signal1, Is.True);
				Assert.That(signal2, Is.True);
			}
		}

		[Test]
		public void Handler_should_react_when_a_registered_message_type_is_received_for_unnamed_endpoint()
		{
			using (var receiverNode = _receiver.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();


				_sender.SendMessage(new RedMessage());

				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				Assert.That(colourSignal, Is.True);
			}
		}

		[Test]
		public void Handler_should_react_when_a_registered_message_type_is_received_for_named_endpoint()
		{
			using (var receiverNode = _receiver.TakeFrom(new Endpoint("registered-message-endpoint")))
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				_sender.SendMessage(new RedMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(colourSignal, Is.True);

			}
		}

		[Test]
		public void Handler_should_get_message_with_proper_correlation_id()
		{
			using (var receiverNode = _receiver.Listen())
			{
				var message = new GreenWhiteMessage();
				TwoColourMessageHandler.Prepare();
				receiverNode.Handle<ITwoColoursMessage>().With<TwoColourMessageHandler>();


				_sender.SendMessage(message);
				var colourSignal = TwoColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.True, "Did not get message!");
				Assert.That(TwoColourMessageHandler.ReceivedMessage.CorrelationId, Is.EqualTo(message.CorrelationId));
			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_unnamed_endpoint()
		{
			using (var receiverNode = _receiver.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				_sender.SendMessage(new JokerMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.False);

			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_named_endpoint()
		{
			ColourMessageHandler.AutoResetEvent.Reset();
			using (var receiverNode = _receiver.TakeFrom(new Endpoint("unregistered-message-endpoint")))
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				_sender.SendMessage(new JokerMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.False);

			}
		}

		[Test, Ignore("this doesn't work local-to-local right now")]
		public void Only_one_handler_should_fire_when_competing_for_an_endpoint()
		{
			using (var namedReceiverNode1 = _receiver.TakeFrom(new Endpoint("shared-endpoint")))
			using (var namedReceiverNode2 = _receiver.TakeFrom(new Endpoint("shared-endpoint")))
			{
				namedReceiverNode1.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
				namedReceiverNode2.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

				_sender.SendMessage(new BatmanMessage());
				var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
				var villanSignal = VillainMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(superheroSignal || villanSignal, Is.True);
				Assert.That(superheroSignal && villanSignal, Is.False);

			}
		}

		[Test]
		public void Should_use_all_registered_handlers_when_a_message_is_received()
		{
			using (var receiverNode = _receiver.Listen())
			{
				receiverNode.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
				receiverNode.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

				_sender.SendMessage(new JokerMessage());
				var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(superheroSignal, Is.True);
				Assert.That(villainSignal, Is.True);

			}
		}

		[Test]
		public void Handler_which_sends_a_new_message_should_get_that_message_handled()
		{
			using (var receiverNode = _receiver.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<ChainHandler>();
				receiverNode.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

				// bug: Current problem in new threading:
				// two messages are being sent, but only one is being picked up.

				_sender.SendMessage(new GreenMessage());

				var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(villainSignal, Is.True);
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }

	}
}