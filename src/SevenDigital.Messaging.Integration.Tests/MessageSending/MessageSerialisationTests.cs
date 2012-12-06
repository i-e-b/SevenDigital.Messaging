using System;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class MessageSerialisationTests
	{
		INodeFactory node_factory;

		protected TimeSpan LongInterval { get { return TimeSpan.FromMinutes(2); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		HoldingEventHook event_hook;
	    private ISenderNode senderNode;
		
		[TestFixtureSetUp]
		public void StartMessaging()
		{
			Helper.SetupTestMessaging();
		}
		[TestFixtureTearDown]
		public void Teardown()
		{
			Console.WriteLine("Cleaning queues");
			Helper.RemoveAllRoutingFromThisSession();
		}

		[SetUp]
		public void SetUp()
		{
			event_hook = new HoldingEventHook();

			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(event_hook));

			node_factory = ObjectFactory.GetInstance<INodeFactory>();
	        senderNode = ObjectFactory.GetInstance<ISenderNode>();
		}
		
		[Test]
		public void Sent_and_received_messages_should_be_equal ()
		{
			using (var receiverNode = node_factory.Listen())
			{
				var message = new GreenMessage();

				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
				
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
