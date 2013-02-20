using System;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class ReceiverNode : IReceiverNode
	{
		readonly IRoutingEndpoint endpoint;
		readonly INode node;

		public ReceiverNode(IRoutingEndpoint endpoint)
		{
			this.endpoint = endpoint;
			node = ObjectFactory.GetInstance<INode>();
			node.SetEndpoint(endpoint);
		}

		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			if ( ! typeof(T).IsInterface) throw new ArgumentException("Handler type must be an interface that implements IMessage");
			return new HandlerTriggering<T>(node);
		}

		public string DestinationName { get { return endpoint.ToString(); } }

		public void Unregister<T>()
		{
            node.RemoveHandler<T>();
		}

		#region Equality members
		
		public bool Equals(ReceiverNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return  Equals(other.endpoint, endpoint);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ReceiverNode)) return false;
			return Equals((ReceiverNode)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (endpoint != null ? endpoint.GetHashCode() : 0);
			}
		}

		public void Dispose()
		{
			node.Dispose();
		}

		#endregion
	}
}