using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class UnregisteringHandlersTests
	{
		INodeFactory _nodeFactory;
		private ISenderNode _senderNode;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(20); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[SetUp]
		public void SetUp()
		{
			Helper.SetupTestMessaging();
			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_nodeFactory = MessagingSystem.Receiver();
			_senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void can_deregister_a_handler_causing_no_further_messages_to_be_processed()
		{
			UnregisterSample.handledTimes = 0;
			using (var receiverNode = _nodeFactory.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<UnregisterSample>();
				_senderNode.SendMessage(new RedMessage());

				Thread.Sleep(250);
				receiverNode.Unregister<UnregisterSample>();
				Thread.Sleep(50);
				_senderNode.SendMessage(new RedMessage());

				Thread.Sleep(250);
				Assert.That(UnregisterSample.handledTimes, Is.EqualTo(1));

				receiverNode.Handle<IColourMessage>().With<UnregisterSample>();
				_senderNode.SendMessage(new RedMessage());
				Thread.Sleep(250);
				Assert.That(UnregisterSample.handledTimes, Is.EqualTo(2));
			}
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }


		public class UnregisterSample : IHandle<IColourMessage>
		{
			public static int handledTimes = 0;

			public void Handle(IColourMessage message)
			{
				Interlocked.Increment(ref handledTimes);
			}
		}

	}
}