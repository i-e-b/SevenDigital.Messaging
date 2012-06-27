using MassTransit;
using SevenDigital.Messaging.Domain;

namespace SevenDigital.Messaging.Services
{
	public class SenderNode : ISenderNode
	{
		readonly Node _node;

		public SenderNode(IMessagingHost host, Endpoint endpoint, IServiceBusFactory serviceBusFactory)
		{
			_node = new Node(host, endpoint, serviceBusFactory);
		}

		public void SendMessage<T>(T message) where T : class, IMessage
		{
			_node.EnsureConnection().Publish(message);
		}

		public bool Equals(SenderNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._node, _node);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (SenderNode)) return false;
			return Equals((SenderNode) obj);
		}

		public override int GetHashCode()
		{
			return (_node != null ? _node.GetHashCode() : 0);
		}

		public void Dispose()
		{
			_node.Dispose();
		}
	}
}