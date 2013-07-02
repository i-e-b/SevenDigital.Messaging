using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.MessageReceiving.Testing;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class ReceiverTests
	{
		Receiver _subject;
		IUniqueEndpointGenerator _endpointGenerator;
		IHandlerManager _handlerManager;
		IMessageRouter _messageRouter;
		IPollingNodeFactory _pollerFactory;
		IDispatcherFactory _dispatchFactory;

		[SetUp]
		public void setup()
		{
			_endpointGenerator = Substitute.For<IUniqueEndpointGenerator>();
			_handlerManager = Substitute.For<IHandlerManager>();
			_messageRouter = Substitute.For<IMessageRouter>();
			_pollerFactory = Substitute.For<IPollingNodeFactory>();
			_dispatchFactory = Substitute.For<IDispatcherFactory>();

			ObjectFactory.Configure(map=>map.For<IHandlerManager>().Use(_handlerManager));
			_endpointGenerator.Generate().Returns(new Endpoint("zoso"));


			_subject = new Receiver(
				_endpointGenerator,
				_messageRouter, _pollerFactory, _dispatchFactory);
		}

		[Test]
		public void take_from_returns_a_new_receiver_bound_to_the_given_endpoint()
		{
			var result = _subject.TakeFrom("an endpoint");

			Assert.That(result, Is.InstanceOf<ReceiverNode>());
			Assert.That(result.DestinationName, Is.EqualTo("an endpoint"));
			Assert.That(SubjectReceiverNodes(), Contains.Item(result));
		}

		[Test]
		public void listen_returns_a_new_receiver_bound_to_a_generated_endpoint()
		{
			var result = _subject.Listen();

			_endpointGenerator.Received().Generate();

			Assert.That(result, Is.InstanceOf<ReceiverNode>());
			Assert.That(result.DestinationName, Is.EqualTo("zoso"));
			Assert.That(SubjectReceiverNodes(), Contains.Item(result));
		}

		[Test]
		public void shutdown_disposes_all_registered_receivers_and_clears_the_receiver_list()
		{
			var nodes = A_set_of_nodes_are_added();

			_subject.Shutdown();

			Nodes_disposed_and_list_empty(nodes);
		}

		[Test]
		public void dispose_disposes_all_registered_receivers_and_clears_the_receiver_list()
		{
			var nodes = A_set_of_nodes_are_added();

			_subject.Dispose();

			Nodes_disposed_and_list_empty(nodes);
		}

		[Test]
		public void removing_a_non_registered_node_does_nothing()
		{
			var nodes = A_set_of_nodes_are_added();
			_subject.Remove(Substitute.For<IReceiverNode>());
			Assert.That(SubjectReceiverNodes(), Is.EquivalentTo(nodes));
		}

		[Test]
		public void after_removing_a_registered_node_it_is_no_longer_listed_but_is_not_disposed()
		{
			var nodes = A_set_of_nodes_are_added();
			_subject.Remove(nodes[0]);
			
			Assert.That(SubjectReceiverNodes(), Is.EquivalentTo(nodes.Skip(1)));
			nodes[0].DidNotReceive().Dispose();
		}

		[Test]
		public void when_concurrent_handler_count_is_set_each_node_is_updated ()
		{
			var nodes = A_set_of_nodes_are_added();
			var new_value = 100;

			_subject.SetConcurrentHandlers(new_value);

			foreach (var node in nodes)
			{
				node.Received().SetConcurrentHandlers(new_value);
			}
		}

		[Test]
		public void if_PurgeOnConnect_is_set_then_TakeFrom_creates_and_purges_the_endpoint ()
		{
			_subject.PurgeOnConnect = true;
			_subject.TakeFrom("an endpoint");

			_messageRouter.Received().AddDestination("an endpoint");
			_messageRouter.Received().Purge("an endpoint");
		}
		
		[Test]
		public void if_PurgeOnConnect_is_not_set_then_Listen_does_not_purge ()
		{
			_subject.Listen();

			_messageRouter.DidNotReceive().Purge(Arg.Any<string>());
		}
		
		[Test]
		public void if_PurgeOnConnect_is_set_then_Listen_creates_and_purges_the_endpoint ()
		{
			_subject.PurgeOnConnect = true;
			_subject.Listen();

			_messageRouter.Received().AddDestination("zoso");
			_messageRouter.Received().Purge("zoso");
		}
		
		[Test]
		public void if_PurgeOnConnect_is_not_set_then_TakeFrom_does_not_purge ()
		{
			_subject.TakeFrom("an endpoint");

			_messageRouter.DidNotReceive().Purge("an endpoint");
		}

		[Test]
		[TestCase("MyProject.Important.Queue", false)]
		[TestCase("permanent_test_listener", false)]
		[TestCase("disintegration.test", false)]
		[TestCase("this.integration.test", true)]
		[TestCase("sevendigital.messaging_listener", true)]
		[TestCase("test_listener", false)]
		[TestCase("test_listener_", true)]
		[TestCase("test_listener_asjkhsdfjkshdf", true)]
		public void delete_filter_cases (string name, bool shouldDelete)
		{
			Assert.That(_subject.DeleteNameFilter(name), Is.EqualTo(shouldDelete));
		}

		[Test]
		public void if_DeleteIntegrationEndpointsOnShutdown_is_set_should_remove_routing_on_shutdown ()
		{
			_subject.DeleteIntegrationEndpointsOnShutdown = true;
			_subject.Shutdown();

			_messageRouter.Received().RemoveRouting(_subject.DeleteNameFilter);
		}
		[Test]
		public void if_DeleteIntegrationEndpointsOnShutdown_is_set_should_remove_routing_on_disposal ()
		{
			_subject.DeleteIntegrationEndpointsOnShutdown = true;
			_subject.Dispose();

			_messageRouter.Received().RemoveRouting(_subject.DeleteNameFilter);
		}
		
		[Test]
		public void if_DeleteIntegrationEndpointsOnShutdown_is_NOT_set_should_remove_routing_on_shutdown ()
		{
			_subject.DeleteIntegrationEndpointsOnShutdown = false;
			_subject.Shutdown();

			_messageRouter.DidNotReceive().RemoveRouting(Arg.Any<Func<string,bool>>());
		}
		[Test]
		public void if_DeleteIntegrationEndpointsOnShutdown_is_NOT_set_should_remove_routing_on_disposal ()
		{
			_subject.DeleteIntegrationEndpointsOnShutdown = false;
			_subject.Dispose();
			
			_messageRouter.DidNotReceive().RemoveRouting(Arg.Any<Func<string,bool>>());
		}

		void Nodes_disposed_and_list_empty(IEnumerable<IReceiverNode> nodes)
		{
			foreach (var node in nodes)
			{
				node.Received().Dispose();
			}

			Assert.That(SubjectReceiverNodes(), Is.Empty);
		}
		IReceiverNode[] A_set_of_nodes_are_added()
		{
			var x = new []	{ Substitute.For<IReceiverNode>()
							, Substitute.For<IReceiverNode>()
							, Substitute.For<IReceiverNode>()
							};

			foreach (var receiverNode in x)
			{
				SubjectReceiverNodes().Add(receiverNode);
			}
			return x;
		}
		ConcurrentBag<IReceiverNode> SubjectReceiverNodes() { return ((IReceiverTesting)_subject).CurrentNodes() as ConcurrentBag<IReceiverNode>; }
	}
}