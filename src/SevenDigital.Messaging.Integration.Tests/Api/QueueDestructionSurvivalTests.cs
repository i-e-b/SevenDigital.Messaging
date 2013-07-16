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
		IReceiver _receiver;
		ISenderNode senderNode;

		const string TestQueue = "Test_listener_survival_test_endpoint";

		static TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }

		[SetUp]
		public void Setup()
		{
			Helper.SetupTestMessagingWithoutPurging();
			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_receiver = MessagingSystem.Receiver();
			senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void Listener_endpoint_should_survive_queue_destruction()
		{
			using (_receiver.TakeFrom(
				new Endpoint(TestQueue),
				_=>_.Handle<IColourMessage>().With<ColourMessageHandler>()))
			{
				Helper.DeleteQueue(TestQueue);
				Thread.Sleep(500);

				senderNode.SendMessage(new RedMessage());
				var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

				Assert.That(colourSignal, Is.True);
			}
		}

		[TearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }
	}
}
