﻿using System;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class PurgingQueues
	{
        INodeFactory _nodeFactory;
        private ISenderNode _senderNode;

        protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }
        protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

        [TestFixtureSetUp]
        public void SetUp()
        {
			Helper.SetupTestMessaging();
			new MessagingConfiguration().PurgeAllMessages();

            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
            _senderNode = ObjectFactory.GetInstance<ISenderNode>();

			using (var l = _nodeFactory.Listen()){
				l.Handle<IColourMessage>().With<ColourMessageHandler>();
			}
			_senderNode.SendMessage(new GreenMessage());
        }

		[Test]
		public void Should_not_get_messages_waiting_on_queue_when_starting_a_new_listener()
		{
			using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
                Assert.That(colourSignal, Is.False);
            }
		}
	}
}