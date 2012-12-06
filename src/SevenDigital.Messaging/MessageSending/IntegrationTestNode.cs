using System;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging
{
	public class IntegrationTestNode:INode
	{
		readonly IMessageRouter messageRouter;
		Node baseNode;
		IRoutingEndpoint endpoint;

		public IntegrationTestNode(IMessageRouter messageRouter)
		{
			this.messageRouter = messageRouter;
			baseNode = ObjectFactory.GetInstance<Node>();
		}

		~IntegrationTestNode()
		{
			Dispose();
		}

		public void Dispose()
		{
			lock (this)
			{
				Console.WriteLine("Cleaning node");
				if (baseNode == null) return;
				baseNode.Dispose();
				baseNode = null;
				
				((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting(
					name => name.ToLower().Contains(".integration.")
					);
			}
		}

		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint;
			baseNode.SetEndpoint(endpoint);
		}

		public void SubscribeHandler<T>(Action<T> action) where T : class
		{
// ReSharper disable EmptyGeneralCatchClause
			try { messageRouter.Purge(endpoint.ToString()); } catch { }
			baseNode.SubscribeHandler(action);
// ReSharper restore EmptyGeneralCatchClause
		}
	}
}