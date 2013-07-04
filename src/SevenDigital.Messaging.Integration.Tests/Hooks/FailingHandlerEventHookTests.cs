using System;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class FailingHandlerEventHookTests
	{
		IReceiver node_factory;
		
		protected TimeSpan LongInterval { get { return TimeSpan.FromMinutes(2); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		IEventHook mock_event_hook;
		
		[TestFixtureSetUp]
		public void StartMessaging()
		{
			Helper.SetupTestMessaging();
		}

		[SetUp]
		public void SetUp()
		{
			mock_event_hook = Substitute.For<IEventHook>();

			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(mock_event_hook));

			node_factory = ObjectFactory.GetInstance<IReceiver>();
		}
		
		[Test]
		public void Should_trigger_failure_hook_when_handler_throws_exception ()
		{
			using (var receiverNode = node_factory.Listen(_=> { }))
			{
				var message = TriggerFailingHandler(receiverNode);

				mock_event_hook.Received().HandlerFailed(
					Arg.Is<IMessage>(im=> im.CorrelationId == message.CorrelationId),
					Arg.Is<Type>(t=>t == typeof (FailingColourHandler)),
					Arg.Is<Exception>(e=>e.Message == FailingColourHandler.Message)
					);
			}
		}

		[Test]
		public void Should_not_trigger_received_hook_when_handler_throws_exception ()
		{
			using (var receiverNode = node_factory.Listen(_=> { }))
			{
				TriggerFailingHandler(receiverNode);

				mock_event_hook.DidNotReceive().MessageReceived(Arg.Any<IMessage>());
			}
		}
		
		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }

		GreenMessage TriggerFailingHandler(IReceiverNode receiverNode)
		{
			receiverNode.Register(new Binding().Handle<IColourMessage>().With<FailingColourHandler>());

			var message = new GreenMessage();
			var senderNode = ObjectFactory.GetInstance<ISenderNode>();

			senderNode.SendMessage(message);

			Assert.That(FailingColourHandler.AutoResetEvent.WaitOne(LongInterval));
			Thread.Sleep(100);
			return message;
		}
	}
}
