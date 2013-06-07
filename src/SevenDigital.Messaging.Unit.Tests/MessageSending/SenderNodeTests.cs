using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending
{
	[TestFixture]
	public class SenderNodeTests
	{
		ISenderNode _subject;
		IMessagingBase _messagingBase;
		IDispatcherFactory _dispatcherFactory;
		ISleepWrapper _sleeper;

		[SetUp]
		public void setup()
		{
			_messagingBase = Substitute.For<IMessagingBase>();
			_dispatcherFactory = Substitute.For<IDispatcherFactory>();
			_sleeper = Substitute.For<ISleepWrapper>();

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
			Assert.Inconclusive();
		}

		[Test]
		public void sender_uses_an_InMemory_queue ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void dispatcher_work_item_is_SendWaitingMessage ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void send_waiting_message_fires_event_hooks ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void a_failing_event_hook_does_not_stop_other_hooks_being_fired ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void a_failing_event_hook_does_not_prevent_a_message_being_sent ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void if_messaging_base_fails_to_send_then_the_sender_sleeps_and_requeues_the_message ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void sleeper_is_reset_after_successful_message_sending ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void send_waiting_message_sends_message_object_through_messaging_base ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void disposing_of_the_sender_node_stops_the_dispatcher ()
		{
			_subject.Dispose();
			Assert.Inconclusive();
		}

		[Test]
		public void sending_a_message_adds_work_to_the_dispatcher ()
		{
			Assert.Inconclusive();
		}

	}
}