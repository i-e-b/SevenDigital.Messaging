using System;// ReSharper disable InconsistentNaming
using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
    [TestFixture]
    public class SendingAndReceivingTests
    {
        INodeFactory _nodeFactory;
        private ISenderNode _senderNode;

        protected TimeSpan LongInterval { get { return TimeSpan.FromMinutes(2); } }
        protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

        [SetUp]
        public void SetUp()
        {
			Helper.SetupTestMessaging();
            ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
            _senderNode = ObjectFactory.GetInstance<ISenderNode>();
        }

		[TestFixtureTearDown]
		public void Teardown()
		{
			// ReSharper disable EmptyGeneralCatchClause
			Console.WriteLine("Cleaning queues");
			try
			{
				var api = Helper.GetManagementApi();
				api.DeleteQueue("registered-message-endpoint");
				api.DeleteQueue("unregistered-message-endpoint");
				api.DeleteQueue("shared-endpoint");
			}
			catch
			{
			}
			try
			{
				var api = Helper.GetManagementApi();
				var queues = api.ListQueues().Where(q => q.name.Contains("_SevenDigital.Messaging.Base_"));
				foreach (var rmQueue in queues)
				{
					api.DeleteQueue(rmQueue.name);
				}
			}
			catch
			{
			}
// ReSharper restore EmptyGeneralCatchClause
		}

	    [Test]
        public void Handler_should_react_when_a_registered_message_type_is_received_for_unnamed_endpoint()
        {
            using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();


                _senderNode.SendMessage(new RedMessage());

                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
                Assert.That(colourSignal, Is.True);
            }
        }

        [Test]
        public void Handler_should_react_when_a_registered_message_type_is_received_for_named_endpoint()
        {
            using (var receiverNode = _nodeFactory.TakeFrom(new Endpoint("registered-message-endpoint")))
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

                _senderNode.SendMessage(new RedMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(colourSignal, Is.True);

            }
        }

        [Test]
        public void Handler_should_get_message_with_proper_correlation_id()
        {
            using (var receiverNode = _nodeFactory.Listen())
            {
                var message = new GreenWhiteMessage();
				TwoColourMessageHandler.Prepare();
                receiverNode.Handle<ITwoColoursMessage>().With<TwoColourMessageHandler>();


                _senderNode.SendMessage(message);
                var colourSignal = TwoColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

                Assert.That(colourSignal, Is.True, "Did not get message!");
                Assert.That(TwoColourMessageHandler.ReceivedMessage.CorrelationId, Is.EqualTo(message.CorrelationId));
            }
        }

        [Test]
        public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_unnamed_endpoint()
        {
            using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

                _senderNode.SendMessage(new JokerMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

                Assert.That(colourSignal, Is.False);

            }
        }

        [Test]
        public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_named_endpoint()
        {
            using (var receiverNode = _nodeFactory.TakeFrom(new Endpoint("unregistered-message-endpoint")))
            {
                receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

                _senderNode.SendMessage(new JokerMessage());
                var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

                Assert.That(colourSignal, Is.False);

            }
        }

        [Test]
        public void Only_one_handler_should_fire_when_competing_for_an_endpoint()
        {
            using (var namedReceiverNode1 = _nodeFactory.TakeFrom(new Endpoint("shared-endpoint")))
            using (var namedReceiverNode2 = _nodeFactory.TakeFrom(new Endpoint("shared-endpoint")))
            {
                namedReceiverNode1.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
                namedReceiverNode2.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

                _senderNode.SendMessage(new BatmanMessage());
                var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(ShortInterval);
                var villanSignal = VillainMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

                Assert.That(superheroSignal || villanSignal, Is.True);
                Assert.That(superheroSignal && villanSignal, Is.False);

            }
        }

        [Test]
        public void Should_use_all_registered_handlers_when_a_message_is_received()
        {
            using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
                receiverNode.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

                _senderNode.SendMessage(new JokerMessage());
                var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(LongInterval);
                var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(superheroSignal, Is.True);
                Assert.That(villainSignal, Is.True);

            }
        }

        [Test]
        public void Handler_which_sends_a_new_message_should_get_that_message_handled()
        {
            using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IColourMessage>().With<ChainHandler>();
                receiverNode.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

                _senderNode.SendMessage(new GreenMessage());
                var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

                Assert.That(villainSignal, Is.True);
            }
        }

    }
}