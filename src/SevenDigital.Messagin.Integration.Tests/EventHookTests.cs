using System;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class EventHookTests
	{
		INodeFactory node_factory;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		Mock<IEventHook> mock_event_hook;

		[TestFixtureSetUp]
		public void SetUp()
		{
			new MessagingConfiguration().WithDefaults();

			mock_event_hook = new Mock<IEventHook>();

			ObjectFactory.Configure(map=> map.For<IServiceBusFactory>().Use<IntegrationTestServiceBusFactory>());
			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(mock_event_hook.Object));

			node_factory = ObjectFactory.GetInstance<INodeFactory>();
		}

		[Test]
		public void Sender_should_trigger_event_hook_with_message_when_sending()
		{
			var message = new GreenMessage();
			var senderNode = node_factory.Sender();
			senderNode.SendMessage(message);

			mock_event_hook.Verify(h => h.MessageSent(message));
		}
		
		[Test]
		public void Should_trigger_event_store_hook_with_message_when_receiving_a_message ()
		{
			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
				var senderNode = node_factory.Sender();
				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				mock_event_hook.Verify(h=>h.MessageReceived(It.Is<IMessage>(im=> im.CorrelationId == message.CorrelationId)));
			}
		}

		[Test]
		public void Every_handler_should_trigger_event_store_hook ()
		{
			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
				receiverNode.Handle<IColourMessage>().With<AnotherColourMessageHandler>();
				var senderNode = node_factory.Sender();
				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				AnotherColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				mock_event_hook.Verify(h=>h.MessageReceived(It.Is<IMessage>(im=> im.CorrelationId == message.CorrelationId)),
					Times.Exactly(2));
			}
		}
	}
}
