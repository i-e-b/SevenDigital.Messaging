using System.Collections.Generic;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard node factory for messaging.
	/// You don't need to create this yourself, use `Messaging.Receiver()`
	/// </summary>
	/// <remarks>
	/// The Receiver is a factory class for ReceiverNodes. It provides
	/// the API point to decide to use a unique endpoint name (Listen) or
	/// a specific endpoint name (TakeFrom)
	/// </remarks>
	public class Receiver : IReceiver, IReceiverControl
	{
		readonly IUniqueEndpointGenerator _uniqueEndPointGenerator;
		readonly ISleepWrapper _sleeper;
		readonly IMessagingBase _messageBase;
		readonly IHandlerManager _handler;
		readonly List<IReceiverNode> _registeredNodes;
		readonly IMessageRouter _messageRouter;
		readonly object _lockObject;

		/// <summary>
		/// Create a new node factory.
		/// You don't need to create this yourself, use `Messaging.Receiver()`
		/// </summary>
		public Receiver(
			IUniqueEndpointGenerator uniqueEndPointGenerator,
			ISleepWrapper sleeper,
			IMessagingBase messageBase,
			IHandlerManager handler,
			IMessageRouter messageRouter)
		{
			_messageRouter = messageRouter;
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
			_sleeper = sleeper;
			_messageBase = messageBase;
			_handler = handler;
			_lockObject = new object();
			_registeredNodes = new List<IReceiverNode>();
			PurgeOnConnect = false;
			DeleteIntegrationEndpointsOnShutdown = false;
		}

		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			lock (_lockObject)
			{
				var node = new ReceiverNode(this, endpoint, _handler, _messageBase, _sleeper);
				_registeredNodes.Add(node);

				if (PurgeOnConnect) PurgeEndpoint(endpoint);
				return node;
			}
		}

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		public IReceiverNode Listen()
		{
			return TakeFrom(_uniqueEndPointGenerator.Generate());
		}

		/// <summary>
		/// Close all receiver nodes that have been created
		/// </summary>
		public void Shutdown()
		{
			lock (_lockObject)
			{
				foreach (var node in _registeredNodes)
				{
					node.Dispose();
				}
				_registeredNodes.Clear();
			}
		}

		/// <summary>
		/// Unregister a node from the shutdown list
		/// </summary>
		public void Remove(IReceiverNode node)
		{
			lock (_lockObject)
			{
				if (_registeredNodes.Contains(node))
					_registeredNodes.RemoveAll(n=> Equals(n, node));
			}
		}

		/// <summary>
		/// Set maximum concurrent handlers per receiver node
		/// </summary>
		public void SetConcurrentHandlers(int max)
		{
			lock (_lockObject)
			{
				foreach (var node in _registeredNodes)
				{
					node.SetConcurrentHandlers(max);
				}
			}
		}

		/// <summary>
		/// Set purging policy. If true, all waiting messages are DELETED when a handler is registered.
		/// This setting is meant for integration tests.
		/// </summary>
		public bool PurgeOnConnect { get; set; }

		/// <summary>
		/// Set cleanup policy. If true, all endpoints generated in integration mode
		/// are deleted when the receiver is disposed.
		/// Default is false.
		/// </summary>
		public bool DeleteIntegrationEndpointsOnShutdown { get; set; }

		/// <summary>
		/// Shutdown all nodes.
		/// </summary>
		public void Dispose()
		{
			Shutdown();

			if (DeleteIntegrationEndpointsOnShutdown) DeleteIntegrationEndpoints();
		}

		void DeleteIntegrationEndpoints()
		{
			lock (_lockObject)
			{
				((RabbitRouter) ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting(DeleteNameFilter);
			}
		}

		static bool DeleteNameFilter(string queueName)
		{
			var name = queueName.ToLowerInvariant();
			var delete = name.ToLowerInvariant().Contains(".integration.")
				   || name.EndsWith("sevendigital.messaging_listener")
				   || name.StartsWith("test_listener_");
			return delete;
		}

		void PurgeEndpoint(Endpoint endpoint)
		{
			_messageRouter.AddDestination(endpoint.ToString());
			_messageRouter.Purge(endpoint.ToString());
		}
	}
}