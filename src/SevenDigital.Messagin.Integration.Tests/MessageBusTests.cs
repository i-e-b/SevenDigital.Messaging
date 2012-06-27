using System;// ReSharper disable InconsistentNaming
using NUnit.Framework;
using SevenDigital.Messaging.Core;
using SevenDigital.Messaging.Core.Domain;
using SevenDigital.Messaging.Core.Services;
using SevenDigital.Messaging.Integration.Tests.Handlers;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.StructureMap;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class MessageBusTests
	{
		INodeFactory _nodeFactory;

		protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(15); } }
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

		[TestFixtureSetUp]
		public void SetUp()
		{
			ConfigureMessaging.WithDefaults();
			ObjectFactory.Configure(map=> map.For<IServiceBusFactory>().Use<IntegrationTestServiceBusFactory>());
			_nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
		}

		[Test]
		public void Handler_should_react_when_a_registered_message_type_is_received_for_unnamed_endpoint()
		{
			using (var receiverNode = _nodeFactory.Listener())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new RedMessage());

					var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);
					Assert.That(colourSignal, Is.True);
				}
			}
		}

		[Test]
		public void Handler_should_react_when_a_registered_message_type_is_received_for_named_endpoint()
		{
			using (var receiverNode = _nodeFactory.ListenOn(new Endpoint("registered-message-endpoint")))
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new RedMessage());
					var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(LongInterval);

					Assert.That(colourSignal, Is.True);
				}
			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_unnamed_endpoint()
		{
			using (var receiverNode = _nodeFactory.Listener())
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new JokerMessage());
					var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

					Assert.That(colourSignal, Is.False);
				}
			}
		}

		[Test]
		public void Handler_should_not_react_when_an_unregistered_message_type_is_received_for_named_endpoint()
		{
			using (var receiverNode = _nodeFactory.ListenOn(new Endpoint("unregistered-message-endpoint")))
			{
				receiverNode.Handle<IColourMessage>().With<ColourMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new JokerMessage());
					var colourSignal = ColourMessageHandler.AutoResetEvent.WaitOne(ShortInterval);

					Assert.That(colourSignal, Is.False);
				}
			}
		}

		[Test]
		public void Only_one_handler_should_fire_when_competing_for_an_endpoint()
		{
			using (var namedReceiverNode1 = _nodeFactory.ListenOn(new Endpoint("shared-endpoint")))
			using (var namedReceiverNode2 = _nodeFactory.ListenOn(new Endpoint("shared-endpoint")))
			{
				namedReceiverNode1.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
				namedReceiverNode2.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new BatmanMessage());
					var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(LongInterval);
					var villanSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

					Assert.That(superheroSignal || villanSignal, Is.True);
					Assert.That(superheroSignal && villanSignal, Is.False);
				}
			}
		}

		[Test]
		public void Should_use_all_registered_handlers_when_a_message_is_received()
		{
			using (var receiverNode = _nodeFactory.Listener())
			{
				receiverNode.Handle<IComicBookCharacterMessage>().With<SuperHeroMessageHandler>();
				receiverNode.Handle<IComicBookCharacterMessage>().With<VillainMessageHandler>();

				using (var senderNode = _nodeFactory.Sender())
				{
					senderNode.SendMessage(new JokerMessage());
					var superheroSignal = SuperHeroMessageHandler.AutoResetEvent.WaitOne(LongInterval);
					var villainSignal = VillainMessageHandler.AutoResetEvent.WaitOne(LongInterval);

					Assert.That(superheroSignal, Is.True);
					Assert.That(villainSignal, Is.True);
				}
			}
		}
	}
}