using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
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

		static TimeSpan LongInterval { get { return TimeSpan.FromSeconds(30); } }

		[SetUp]
		public void Setup()
		{
			Helper.SetupTestMessaging();
			ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
			nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
			senderNode = ObjectFactory.GetInstance<ISenderNode>();

		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			var api = Helper.GetManagementApi();
			var queues = api.ListQueues();

			if (queues.Any(q=>q.name == TestQueue)) api.DeleteQueue(TestQueue);
			
			var testQueues = queues.Where(q=>q.name.Contains("_SevenDigital.Messaging.Base_"));
			foreach (var rmQueue in testQueues)
			{
				api.DeleteQueue(rmQueue.name);
			}
		}

		[Test, Ignore("A bug in MassTransit is preventing this test from passing")]
		public void Listener_endpoint_should_survive_queue_being_deleted()
		{
			using (var receiverNode = nodeFactory.TakeFrom(new Endpoint(TestQueue)))
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				Helper.GetManagementApi().DeleteQueue(TestQueue);

				Thread.Sleep(LongInterval);

                senderNode.SendMessage(new RedMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(colourSignal, Is.True);

            }
		}
	}
}
