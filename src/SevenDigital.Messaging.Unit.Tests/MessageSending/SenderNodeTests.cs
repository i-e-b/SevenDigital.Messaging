using System;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using DispatchSharp.WorkerPools;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending
{
	[TestFixture]
	public class SenderNodeTests
	{
		ISenderNode _subject;
		IMessagingBase _messagingBase;
		IDispatcherFactory _dispatcherFactory;
		ISleepWrapper _sleeper;
		IDispatch<IMessage> _dispatcher;
		IEventHook _eventHook1, _eventHook2;

		[SetUp]
		public void setup()
		{
			_messagingBase = Substitute.For<IMessagingBase>();
			_sleeper = Substitute.For<ISleepWrapper>();
			_dispatcher = Substitute.For<IDispatch<IMessage>>();
			_dispatcherFactory = Substitute.For<IDispatcherFactory>();
			_dispatcherFactory.Create(Arg.Any<IWorkQueue<IMessage>>(), Arg.Any<IWorkerPool<IMessage>>()).Returns(_dispatcher);

			_eventHook1 = Substitute.For<IEventHook>();
			_eventHook2 = Substitute.For<IEventHook>();
			ObjectFactory.Configure(map => {
				map.For<IEventHook>().Use(_eventHook1);
				map.For<IEventHook>().Use(_eventHook2);
			});

			_subject = new SenderNode(_messagingBase, _dispatcherFactory, _sleeper);
		}

		[Test]
		public void TODO_DispatchSharp_could_do_with_a_failure_policy_or_other_way_to_pause_and_retry_work_after_failures ()
		{
			Assert.Inconclusive("TODO");
		}

		[Test]
		public void creating_a_sender_node_should_create_a_single_threaded_dispatcher ()
		{
			_dispatcherFactory.Received().Create(
				Arg.Any<IWorkQueue<IMessage>>(),
				Arg.Is<ThreadedWorkerPool<IMessage>>(m=>m.PoolSize() == 1) // <-- testing this one
				);
		}

		[Test]
		public void sender_uses_an_InMemory_queue ()
		{
			_dispatcherFactory.Received().Create(
				Arg.Any<InMemoryWorkQueue<IMessage>>(), // <-- testing this one
				Arg.Any<IWorkerPool<IMessage>>()
				);
		}

		[Test]
		public void sending_a_message_adds_work_to_the_dispatcher ()
		{
			var msg = new TestMessage();
			_subject.SendMessage(msg);

			_dispatcher.Received().AddWork(msg);
		}

		[Test]
		public void dispatcher_work_item_is_SendWaitingMessage ()
		{
			_dispatcher.Received().AddConsumer(((SenderNode)_subject).SendWaitingMessage);
		}

		[Test]
		public void send_waiting_message_fires_event_hooks ()
		{
			var msg = new TestMessage();
			((SenderNode)_subject).SendWaitingMessage(msg);

			_eventHook1.Received().MessageSent(msg);
			_eventHook2.Received().MessageSent(msg);
		}

		[Test]
		public void a_failing_event_hook_does_not_stop_other_hooks_being_fired ()
		{
			var msg = new TestMessage();
			_eventHook1.When(m=>m.MessageSent(Arg.Any<IMessage>())).Do(c => { throw new Exception("test exception"); });

			((SenderNode)_subject).SendWaitingMessage(msg);

			_eventHook1.Received().MessageSent(msg);
			_eventHook2.Received().MessageSent(msg);
		}

		[Test]
		public void a_failing_event_hook_does_not_prevent_a_message_being_sent ()
		{
			var msg = new TestMessage();
			_eventHook1.When(m=>m.MessageSent(Arg.Any<IMessage>())).Do(c => { throw new Exception("test exception"); });

			((SenderNode)_subject).SendWaitingMessage(msg);

			_messagingBase.Received().SendMessage(msg);
		}

		[Test]
		public void if_messaging_base_fails_to_send_then_the_sender_sleeps_and_requeues_the_message ()
		{
			var msg = new TestMessage();
			_messagingBase.When(m=>m.SendMessage(Arg.Any<object>())).Do(c => { throw new Exception("test exception"); });

			((SenderNode)_subject).SendWaitingMessage(msg);

			_sleeper.Received().SleepMore();
			_dispatcher.Received().AddWork(msg);
		}

		[Test]
		public void sleeper_is_reset_after_successful_message_sending ()
		{
			var msg = new TestMessage();
			((SenderNode)_subject).SendWaitingMessage(msg);

			_sleeper.Received().Reset();
			_sleeper.DidNotReceive().SleepMore();
			_dispatcher.DidNotReceive().AddWork(msg);
		}

		[Test]
		public void send_waiting_message_sends_message_object_through_messaging_base ()
		{
			var msg = new TestMessage();
			((SenderNode)_subject).SendWaitingMessage(msg);

			_messagingBase.Received().SendMessage(msg);
		}

		[Test]
		public void disposing_of_the_sender_node_stops_the_dispatcher ()
		{
			_subject.Dispose();
			_dispatcher.Received().Stop();
		}
		
		public class TestMessage : IMessage { public Guid CorrelationId { get; set; } }
	}
}