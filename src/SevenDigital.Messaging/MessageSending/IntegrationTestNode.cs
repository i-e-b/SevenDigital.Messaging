using System;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class IntegrationTestNode : INode
	{
		readonly IMessageRouter messageRouter;
		Node baseNode;
		IRoutingEndpoint endpoint;

		public IntegrationTestNode(IMessageRouter messageRouter)
		{
			this.messageRouter = messageRouter;
			baseNode = ObjectFactory.GetInstance<Node>();
		}

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

		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint;
			baseNode.SetEndpoint(endpoint);
		}

		public void SubscribeHandler<T>(HandlerAction<T> action) where T : class, IMessage
		{
// ReSharper disable EmptyGeneralCatchClause
			try { messageRouter.Purge(endpoint.ToString()); } catch { }
			baseNode.SubscribeHandler(action);
// ReSharper restore EmptyGeneralCatchClause
		}
	}
}