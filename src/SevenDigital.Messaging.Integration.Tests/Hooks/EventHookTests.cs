using System;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests._Helpers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Handlers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Hooks
{
	[TestFixture]
	public class EventHookTests
	{
		IReceiver node_factory;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(30); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		IEventHook mock_event_hook;
		private ISenderNode senderNode;


		[SetUp]
		public void SetUp()
		{
			Helper.SetupTestMessaging();
			mock_event_hook = Substitute.For<IEventHook>();

			ObjectFactory.Configure(map => map.For<IEventHook>().Use(mock_event_hook));

			node_factory = MessagingSystem.Receiver();
			senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void Sender_should_trigger_event_hook_with_message_when_sending()
		{
			var message = new GreenMessage();

			senderNode.SendMessage(message);
			MessagingSystem.Control.Shutdown();

			mock_event_hook.Received().MessageSent(Arg.Is<IMessage>(m=> m.CorrelationId == message.CorrelationId));
		}

		[Test]
		public void Should_trigger_event_hook_with_message_when_receiving_a_message()
		{
			var message = new GreenMessage();
			using (node_factory.Listen(_=>_.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				senderNode.SendMessage(message);

				Assert.That(ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval));
			}
			lock (mock_event_hook)
			{
				mock_event_hook.Received().MessageReceived(Arg.Is<IColourMessage>(im => im.CorrelationId == message.CorrelationId));
			}
		}

		[Test]
		public void Should_trigger_event_hook_with_message_when_receiving_a_message_from_a_base_type()
		{
			var message = new GreenMessage();
			using (node_factory.Listen(_=>_.Handle<IMessage>().With<GenericHandler>()))
			{
				senderNode.SendMessage(message);

				GenericHandler.AutoResetEvent.WaitOne(LongInterval);
			}
			lock (mock_event_hook)
			{
				mock_event_hook.Received().MessageReceived(Arg.Is<IColourMessage>(im => im.CorrelationId == message.CorrelationId));
			}
		}

		[Test]
		public void Every_handler_should_trigger_event_hook()
		{
			using (node_factory.Listen(_=>_
				.Handle<IColourMessage>().With<ColourMessageHandler>()
				.Handle<IColourMessage>().With<AnotherColourMessageHandler>()
				))
			{
				var message = new GreenMessage();
				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
				AnotherColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

				mock_event_hook.Received(2).MessageReceived(Arg.Is<IMessage>(im => im.CorrelationId == message.CorrelationId));
			}
		}

		[TearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}

	public class GenericHandler : IHandle<IMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IMessage message)
		{
			AutoResetEvent.Set();
		}
	}
}
