using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

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
			ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
			nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
			senderNode = ObjectFactory.GetInstance<ISenderNode>();

		}

		[Test]
		public void Listener_endpoint_should_survive_connection_being_lost()
		{
			using (var receiverNode = nodeFactory.TakeFrom(new Endpoint(TestQueue)))
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				KillAndRebuildQueue(receiverNode);

                senderNode.SendMessage(new RedMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(colourSignal, Is.True);

            }
		}

		[TestFixtureTearDown]
		public void Stop() { new MessagingConfiguration().Shutdown(); }

		static void KillAndRebuildQueue(IReceiverNode receiverNode)
		{
			Helper.DeleteQueue(TestQueue);
			Thread.Sleep(LongInterval);
			MessagingBase.ResetCaches();
			receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
		}
	}
}
