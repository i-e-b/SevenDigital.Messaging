using System;// ReSharper disable InconsistentNaming
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.MessageReceiving;

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

			MessagingSystem.Events.ClearEventHooks();
			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_receiver = MessagingSystem.Receiver();
			_sender = MessagingSystem.Sender();
		}

		[Test]
		public void Handler_should_react_for_all_message_types_it_is_handling()
		{
			using (_receiver.Listen(_=>_
				.Handle<IColourMessage>().With<AllColourMessagesHandler>()
				.Handle<ITwoColoursMessage>().With<AllColourMessagesHandler>()
				))
			{
				AllColourMessagesHandler.Prepare();
	
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
			using (_receiver.Listen(_=>_
				.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				_sender.SendMessage(new RedMessage());

				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				Assert.That(colourSignal, Is.True);
			}
		}

		[Test]
		public void Handler_should_react_when_a_registered_message_type_is_received_for_named_endpoint()
		{
			using (_receiver.TakeFrom(
				new Endpoint("Test_listener_registered-message-endpoint"),
				_=>_.Handle<IColourMessage>().With<ColourMessageHandler>()
				))
			{
				_sender.SendMessage(new RedMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(colourSignal, Is.True);
			}
		}

		[Test]
		public void Handler_should_get_message_with_proper_correlation_id()
		{
			using (_receiver.Listen(_=>_
				.Handle<ITwoColoursMessage>().With<TwoColourMessageHandler>()))
			{
				var message = new GreenWhiteMessage();
				TwoColourMessageHandler.Prepare();

				_sender.SendMessage(message);
				var colourSignal = TwoColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.True, "Did not get message!");
				Assert.That(TwoColourMessageHandler.ReceivedCount, Is.EqualTo(1), "Got a wrong number of messages");
				Assert.That(TwoColourMessageHandler.ReceivedMessage.CorrelationId, Is.EqualTo(message.CorrelationId));
			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_unnamed_endpoint()
		{
			using (_receiver.Listen(_ => _.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				_sender.SendMessage(new JokerMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.False);
			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_named_endpoint()
		{
			ColourMessageHandler.AutoResetEvent.Reset();
			using (_receiver.TakeFrom(
				new Endpoint("Test_listener_unregistered-message-endpoint"),
				_ => _.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				_sender.SendMessage(new JokerMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				Assert.That(colourSignal, Is.False);
			}
		}

		[Test, Ignore("this doesn't work local-to-local right now")]
		public void Only_one_handler_should_fire_when_competing_for_an_endpoint()
		{
			using (_receiver.TakeFrom("Test_listener_shared-endpoint", _ => _.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>()))
			using (_receiver.TakeFrom("Test_listener_shared-endpoint", _ => _.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>()))
			{
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
			using (_receiver.Listen(_ => _
				.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>()
				.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>()
				))
			{
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
			using (_receiver.Listen(_ => _
				.Handle<IColourMessage>().With<ChainHandler>()
				.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>()
				))
			{
				_sender.SendMessage(new GreenMessage());

				var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(villainSignal, Is.True);
			}
		}

		[Test]
		public void should_be_able_to_register_handlers_with_lot_of_messages_on_a_queue()
		{
			using (var receiverNode = _receiver.Listen(_ => { }))
			{
				for (int i = 0; i < 1000; i++)
				{
					_sender.SendMessage(new JokerMessage());
				}

				receiverNode.Register(
					new Binding()
					.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>()
					.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>()
					);

				var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(superheroSignal, Is.True);
				Assert.That(villainSignal, Is.True);
			}

		}


		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }

	}
}