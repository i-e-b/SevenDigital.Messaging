using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class FailingHandlerEventHookTests
	{
		INodeFactory node_factory;
		
		protected TimeSpan LongInterval { get { return TimeSpan.FromMinutes(2); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		Mock<IEventHook> mock_event_hook;
		
		[TestFixtureSetUp]
		public void StartMessaging()
		{
			Helper.SetupTestMessaging();
		}

		[SetUp]
		public void SetUp()
		{
			mock_event_hook = new Mock<IEventHook>();

			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(mock_event_hook.Object));

			node_factory = ObjectFactory.GetInstance<INodeFactory>();
		}
		
		[Test]
		public void Should_trigger_failure_hook_when_handler_throws_exception ()
		{
			using (var receiverNode = node_factory.Listen())
			{
				var message = TriggerFailingHandler(receiverNode);

				mock_event_hook.Verify(h=>h.HandlerFailed(
					It.Is<IMessage>(im=> im.CorrelationId == message.CorrelationId),
					It.Is<Type>(t=>t == typeof (FailingColourHandler)),
					It.Is<Exception>(e=>e.Message == FailingColourHandler.Message)
					));
			}
		}


		[Test]
		public void Should_not_trigger_received_hook_when_handler_throws_exception ()
		{
			using (var receiverNode = node_factory.Listen())
			{
				TriggerFailingHandler(receiverNode);

				mock_event_hook.Verify(h=>h.MessageReceived(It.IsAny<IMessage>()),
					Times.Exactly(0));
			}
		}

		GreenMessage TriggerFailingHandler(IReceiverNode receiverNode)
		{
			receiverNode.Handle<IColourMessage>().With<FailingColourHandler>();

			var message = new GreenMessage();
			var senderNode = ObjectFactory.GetInstance<ISenderNode>();

			senderNode.SendMessage(message);

			Assert.That(FailingColourHandler.AutoResetEvent.WaitOne(LongInterval));
			Thread.Sleep(100);
			return message;
		}
	}
}
