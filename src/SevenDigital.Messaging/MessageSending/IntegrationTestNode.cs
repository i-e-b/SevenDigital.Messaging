using System;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Integration test node.
	/// You don't need to create this yourself, use `Messaging.Receiver()` in integration test mode
	/// </summary>
	public class IntegrationTestNode : INode
	{
		readonly IMessageRouter messageRouter;
		Node baseNode;
		IRoutingEndpoint endpoint;

		/// <summary>
		/// Create a new integration test node
		/// </summary>
		public IntegrationTestNode(IMessageRouter messageRouter)
		{
			this.messageRouter = messageRouter;
			baseNode = ObjectFactory.GetInstance<Node>();
		}

		/// <summary>
		/// Shutdown base node and remove test endpoints
		/// </summary>
		public void Dispose()
		{
			lock (this)
			{
				Console.WriteLine("Cleaning node");
				if (baseNode == null) return;
				baseNode.Dispose();
				baseNode = null;

				// allow removal of any integration test routes, plus all the specific tests in the 
				// messaging framework itself
				((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting(
					name => name.ToLower().Contains(".integration.")
						|| name == "ping-pong-endpoint" || name == "registered-message-endpoint"
						|| name == "shared-endpoint" || name == "unregistered-message-endpoint"
						|| name == "survival_test_endpoint"
						|| name.EndsWith("sevendigital.messaging_listener")
						|| name.StartsWith("testlistener_")
					);
			}
		}

		/// <summary>
		/// Set this node to listen on a messaging endpoint.
		/// </summary>
		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint;
			baseNode.SetEndpoint(endpoint);
		}

		/// <summary>
		/// Bind a message type/handler type pair to consume messages on this node
		/// </summary>
		public void SubscribeHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>
		{
			// ReSharper disable EmptyGeneralCatchClause
			try { messageRouter.Purge(endpoint.ToString()); }
			catch { }
			baseNode.SubscribeHandler<TMessage, THandler>();
			// ReSharper restore EmptyGeneralCatchClause
		}

		/// <summary>
		/// Unbind a handler from all messages
		/// </summary>
		public void RemoveHandler<T>()
		{
			baseNode.RemoveHandler<T>();
		}
	}
}