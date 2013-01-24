using System;
using Moq;
using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
	[TestFixture]
	public class LoopbackConfigurationTests
	{
		Mock<IEventHook> mock_event_hook;
	    private ISenderNode _senderNode;
	    private INodeFactory _nodeFactory;

	    [SetUp]
		public void When_configuring_with_loopback_even_if_default_configuration_used_afterwards ()
		{
			new MessagingConfiguration().WithLoopback();
		    new MessagingConfiguration().WithDefaults();
			
			mock_event_hook = new Mock<IEventHook>();
			ObjectFactory.Configure(map=> map.For<IEventHook>().Use(mock_event_hook.Object));

			ResetHandlers();

            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
            _senderNode = ObjectFactory.GetInstance<ISenderNode>();
		}

		static void ResetHandlers()
		{
			DummyHandler.Reset();
			OtherHandler.Reset();
			DifferentHandler.Reset();
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_without_waiting ()
		{
		    using (var receiver = _nodeFactory.Listen())
			{
				receiver.Handle<IDummyMessage>().With<DummyHandler>();
				_senderNode.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

				Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void Should_provide_thrown_exception_from_failing_handler()
		{
			using (var receiver = _nodeFactory.Listen())
			{
				receiver.Handle<IDummyMessage>().With<CrappyHandler>();
				_senderNode.SendMessage(new DummyMessage { CorrelationId = Guid.NewGuid() });

				mock_event_hook.Verify(h=>h.HandlerFailed(It.IsAny<IMessage>(),
					It.Is<Type>(t=>t == typeof(CrappyHandler)),
					It.Is<ArgumentException>(ex => ex.GetType() == typeof(ArgumentException))));
			}
		}

	    [Test]
		public void Should_receive_messages_at_every_applicable_handler ()
		{
			using (var receiver = _nodeFactory.Listen())
			{
				receiver.Handle<IDummyMessage>().With<DummyHandler>();
				receiver.Handle<IDummyMessage>().With<OtherHandler>();
				receiver.Handle<IDifferentMessage>().With<DifferentHandler>();

				_senderNode.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

				Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
				Assert.That(OtherHandler.CallCount, Is.EqualTo(1));
				Assert.That(DifferentHandler.CallCount, Is.EqualTo(0));
			}
		}

		[Test]
		public void Should_receive_competing_messages_at_only_one_handler ()
		{
			var receiver1 = _nodeFactory.TakeFrom("Compete");
			var receiver2 = _nodeFactory.TakeFrom("Compete");

			receiver1.Handle<IDummyMessage>().With<DummyHandler>();
			receiver2.Handle<IDummyMessage>().With<DummyHandler>();

			_senderNode.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

			receiver1.Dispose();
			receiver2.Dispose();

			Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
		}

		[Test]
		public void Should_fire_sent_and_received_event_hooks ()
		{
			using (var receiver = _nodeFactory.Listen())
			{
				receiver.Handle<IDummyMessage>().With<DummyHandler>();
				_senderNode.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});
			}

			mock_event_hook.Verify(h => h.MessageSent(It.IsAny<DummyMessage>()));
			mock_event_hook.Verify(h => h.MessageReceived(It.IsAny<DummyMessage>()));
		}
		
		[Test]
		public void Should_fire_failure_hook_on_failure ()
		{
			using (var receiver = _nodeFactory.Listen())
			{
				receiver.Handle<IDummyMessage>().With<CrappyHandler>();
				_senderNode.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});
			}

			mock_event_hook.Verify(h=>h.MessageSent(It.IsAny<DummyMessage>()));
			mock_event_hook.Verify(h=>h.HandlerFailed(It.IsAny<DummyMessage>(), It.Is<Type>(t=> t == typeof(CrappyHandler)), It.IsAny<Exception>()));
		}

        [Test]
        public void Should_get_registered_listener_for_type()
        {
            using (var receiver = _nodeFactory.Listen())
            {
                receiver.Handle<IMessage>().With<AHandler>();
                var listeners = new MessagingConfiguration().LoopbackListenersForMessage<IMessage>();

                Assert.That(listeners, Contains.Item(typeof (AHandler)));
            }
        }
	}

	public class CrappyHandler:IHandle<IDummyMessage>
	{
		public void Handle(IDummyMessage message)
		{
			throw new ArgumentException("I failed");
		}
	}

	public class DifferentHandler:IHandle<IDifferentMessage>
	{
		public static int CallCount;
		public void Handle(IDifferentMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public interface IDifferentMessage:IMessage
	{
	}

	public class OtherHandler:IHandle<IDummyMessage>
	{
		public static int CallCount;
		public void Handle(IDummyMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public class DummyMessage:IDummyMessage
	{
		public Guid CorrelationId { get; set; }
	}

	public class DummyHandler:IHandle<IDummyMessage>
	{
		public static int CallCount;
		public void Handle(IDummyMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public interface IDummyMessage:IMessage
	{
	}
}
