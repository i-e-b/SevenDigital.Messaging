﻿using System;
using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class SubscribeHandlerTests
	{
		INode subject;
		Mock<IMessagingBase> messagingBase;
		Mock<IDestinationPoller> destinationPoller;
		Mock<IMessageDispatcher> messageDispatcher;
		Action<IMessage> myAction;
		string destinationName = "woop";

		[SetUp]
		public void Subscribing_a_handler_with_a_message_dispatcher ()
		{
			messagingBase = new Mock<IMessagingBase>();
			destinationPoller = new Mock<IDestinationPoller>();
			messageDispatcher = new Mock<IMessageDispatcher>();
			subject = new Node(messagingBase.Object, messageDispatcher.Object, destinationPoller.Object);
			subject.SetEndpoint(new Endpoint(destinationName));

			myAction = msg => { };
			subject.SubscribeHandler(myAction);
		}

		[Test]
		public void Should_ensure_destination_is_set_up_for_node ()
		{
			messagingBase.Verify(m=>m.CreateDestination<IMessage>(destinationName));
		}

		[Test]
		public void Should_add_type_and_action_to_dispatcher ()
		{
			messageDispatcher.Verify(m=>m.AddHandler(myAction));
		}

		[Test]
		public void Should_ensure_destination_poller_is_started ()
		{
			destinationPoller.Verify(m=>m.Start());
		}

		[Test]
		public void Should_make_sure_destination_poller_is_watching_target_destination ()
		{
			destinationPoller.Verify(m=>m.SetDestinationToWatch(destinationName));
		}
	}
}
