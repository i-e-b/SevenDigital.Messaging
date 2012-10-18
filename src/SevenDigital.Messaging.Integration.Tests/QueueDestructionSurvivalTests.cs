using System;
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

		static TimeSpan LongInterval { get { return TimeSpan.FromSeconds(30); } }

		[SetUp]
		public void Setup()
		{
			Helper.SetupTestMessaging();
			ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
			nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
			senderNode = ObjectFactory.GetInstance<ISenderNode>();

		}

		[Test, Ignore("A bug in MassTransit is preventing this test from passing")]
		public void Listener_endpoint_should_survive_queue_being_deleted()
		{
			using (var receiverNode = nodeFactory.TakeFrom(new Endpoint("survival_test_endpoint")))
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				Helper.GetManagementApi().DeleteQueue("survival_test_endpoint");

				Thread.Sleep(LongInterval);

                senderNode.SendMessage(new RedMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(colourSignal, Is.True);

            }
		}
	}
}
