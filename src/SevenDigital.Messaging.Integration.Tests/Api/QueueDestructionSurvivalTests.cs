using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class QueueDestructionSurvivalTests
	{
		INodeFactory nodeFactory;
		ISenderNode senderNode;

		const string TestQueue = "survival_test_endpoint";

		static TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }

		[SetUp]
		public void Setup()
		{
			Helper.SetupTestMessaging();
			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			nodeFactory = MessagingSystem.Receiver();
			senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void Listener_endpoint_should_survive_queue_destruction()
		{
			using (var receiverNode = nodeFactory.TakeFrom(new Endpoint(TestQueue)))
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				Helper.DeleteQueue(TestQueue);
				Thread.Sleep(500);

				senderNode.SendMessage(new RedMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(colourSignal, Is.True);
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}
}
