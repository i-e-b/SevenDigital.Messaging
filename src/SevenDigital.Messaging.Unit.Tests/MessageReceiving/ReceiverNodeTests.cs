using System;
using DispatchSharp;
using DispatchSharp.WorkerPools;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class ReceiverNodeTests
	{
		IReceiverNode _subject;
		IReceiverControl _parent;
		IRoutingEndpoint _endpoint;
		IHandlerManager _handlerManager;
		IPollingNodeFactory _pollerFactory;
		ITypedPollingNode _poller;
		IDispatcherFactory _dispatcherFactory;
		IDispatch<IPendingMessage<object>> _dispatcher;

		[SetUp]
		public void setup()
		{
			_parent = Substitute.For<IReceiverControl>();
			_endpoint = Substitute.For<IRoutingEndpoint>();
			_endpoint.ToString().Returns("endpoint");
			_handlerManager = Substitute.For<IHandlerManager>();

			_poller = Substitute.For<ITypedPollingNode>();
			
			_pollerFactory = Substitute.For<IPollingNodeFactory>();
			_pollerFactory.Create(Arg.Any<IRoutingEndpoint>()).Returns(_poller);

			_dispatcher = Substitute.For<IDispatch<IPendingMessage<object>>>();

			_dispatcherFactory = Substitute.For<IDispatcherFactory>();
			_dispatcherFactory.Create(Arg.Any<IWorkQueue<IPendingMessage<object>>>(), Arg.Any<IWorkerPool<IPendingMessage<object>>>())
				.Returns(_dispatcher);

			_subject = new ReceiverNode(
				_parent, _endpoint, _handlerManager, _pollerFactory, _dispatcherFactory
				);
		}

		[Test]
		public void binding_a_message_type_to_a_handler_type_adds_the_binding_to_the_handler_manager ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_handlerManager.Received().AddHandler(typeof(IMessage), typeof(MessageHandler));
		}

		[Test]
		public void binding_a_message_type_to_a_handler_type_adds_the_type_to_the_polling_node ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_poller.Received().AddMessageType(typeof(IMessage));
		}

		[Test]
		public void unbinding_a_message_type_removes_the_handler_associations ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_subject.Unregister<MessageHandler>();

			_handlerManager.RemoveHandler(typeof(MessageHandler));
		}

		[Test]
		public void disposing_of_the_node_unregisters_the_node_from_parent_and_stops_dispatcher ()
		{
			_subject.Dispose();
			_parent.Received().Remove(_subject);
			_dispatcher.Received().Stop();
		}

		[Test]
		public void destination_name_comes_from_endpoint ()
		{
			Assert.That(_subject.DestinationName, Is.EqualTo("endpoint"));
			_endpoint.Received().ToString();
		}

		[Test]
		public void node_creates_and_starts_a_dispatcher ()
		{
			_dispatcherFactory.ReceivedWithAnyArgs().Create<IPendingMessage<object>>(null,null);
			_dispatcher.Start();
		}

		[Test]
		public void dispatcher_is_set_to_use_HandleIncomingMessage ()
		{
			_dispatcher.Received().AddConsumer(Arg.Is<Action<IPendingMessage<object>>>(a => a == ((ReceiverNode)_subject).HandleIncomingMessage));
		}

		[Test]
		public void dispatcher_uses_a_threaded_worker_pool ()
		{
			_dispatcherFactory.Received().Create(Arg.Any<IWorkQueue<IPendingMessage<object>>>(),
				Arg.Any<ThreadedWorkerPool<IPendingMessage<object>>>());
		}

		[Test]
		public void dispatcher_uses_the_poller_factory_node ()
		{
			_dispatcherFactory.Received().Create(
				_poller,
				Arg.Any<IWorkerPool<IPendingMessage<object>>>());
		}

		[Test]
		public void setting_concurreny_limit_sets_dispatcher_inflight_limit ()
		{
			_dispatcher.WhenForAnyArgs(m => m.MaximumInflight = Arg.Any<int>()).Do(
				ci => Assert.That(ci.Args()[0], Is.EqualTo(2))
				);
			_subject.SetConcurrentHandlers(2);
		}

		[Test]
		public void incoming_messages_are_passed_to_handler_manager ()
		{
			var incoming = Substitute.For<IPendingMessage<object>>();
			((ReceiverNode)_subject).HandleIncomingMessage(incoming);

			_handlerManager.Received().TryHandle(incoming);
		}

		#region TypeJunk

		public class MessageHandler : IHandle<IMessage> { public void Handle(IMessage message) { } }

		#endregion TypeJunk
	}
}