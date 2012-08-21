using System;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture, Ignore()]
	public class ClearingQueueWhenStartingListener
	{
		
        INodeFactory _nodeFactory;
        private ISenderNode _senderNode;

        protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }
        protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

        [TestFixtureSetUp]
        public void When_sending_message_to_queue_without_active_listener()
        {
            new MessagingConfiguration().WithDefaults();
            ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
            _senderNode = ObjectFactory.GetInstance<ISenderNode>();

			RegisterListenerQueue();
			_senderNode.SendMessage(new GreenMessage());
        }

		void RegisterListenerQueue()
		{
			using (var receiverNode = _nodeFactory.Listen())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();
			}
		}


		[Test, Ignore()]
        public void Should_not_have_any_messages_handled ()
        {
            using (var receiverNode = _nodeFactory.PurgeAndListen())
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
                Assert.That(colourSignal, Is.False);
            }
        }
	}
}
