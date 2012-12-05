using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class ReceiverNode : IReceiverNode
	{
		readonly IMessagingHost host;
		readonly IRoutingEndpoint endpoint;
		readonly IMessageDispatch messageDispatch;
		readonly Node node;

		public ReceiverNode(IMessagingHost host, IRoutingEndpoint endpoint, IMessageDispatch messageDispatch)
		{
			this.host = host;
			this.endpoint = endpoint;
			this.messageDispatch = messageDispatch;
			node = new Node(host, endpoint, this.messageDispatch);
		}

		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			return new HandlerTriggering<T>(messageDispatch);
		}

		public void Dispose()
		{
			node.Dispose();
		}

		#region Equality members

		public bool Equals(ReceiverNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.host, host) && Equals(other.endpoint, endpoint);
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
				return ((host != null ? host.GetHashCode() : 0) * 397) ^ (endpoint != null ? endpoint.GetHashCode() : 0);
			}
		}

		#endregion
	}
}