using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	class HoldingEventHook : IEventHook
	{
		public IMessage sent, received;
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void MessageSent(IMessage msg)
		{
			sent = msg;
		}

		public void MessageReceived(IMessage msg)
		{
			received = msg;
			AutoResetEvent.Set();
		}
	}

	[TestFixture]
	public class MessageSerialisationTests
	{
		INodeFactory node_factory;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		HoldingEventHook event_hook;

		[TestFixtureSetUp]
		public void SetUp()
		{
			new MessagingConfiguration().WithDefaults();

			event_hook = new HoldingEventHook();

			ObjectFactory.Configure(map=> map.For<IServiceBusFactory>().Use<IntegrationTestServiceBusFactory>());
			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(event_hook));

			node_factory = ObjectFactory.GetInstance<INodeFactory>();
		}
		
		[Test]
		public void Sent_and_received_messages_should_be_equal ()
		{
			using (var receiverNode = node_factory.Listener())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
				var senderNode = node_factory.Sender();
				senderNode.SendMessage(message);

				ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
				HoldingEventHook.AutoResetEvent.WaitOne(ShortInterval);

				var sent = (IColourMessage)event_hook.sent;
				var received = (IColourMessage)event_hook.received;

				Assert.That(sent, Is.Not.Null, "sent message was null");
				Assert.That(received, Is.Not.Null, "received message was null");

				Assert.That(sent.CorrelationId, Is.EqualTo(received.CorrelationId));
				Assert.That(sent.Text, Is.EqualTo(received.Text));
			}
		}
	}
}
