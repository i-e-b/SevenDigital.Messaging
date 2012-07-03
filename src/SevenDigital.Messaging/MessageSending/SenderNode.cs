using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class SenderNode : ISenderNode
	{
		readonly Node node;

		public SenderNode(IMessagingHost host, IRoutingEndpoint endpoint, IServiceBusFactory serviceBusFactory)
		{
			node = new Node(host, endpoint, serviceBusFactory);
		}

		public virtual void SendMessage<T>(T message) where T : class, IMessage
		{
			node.EnsureConnection().Publish(message, c => {});

			ObjectFactory
				.GetAllInstances<IEventHook>()
				.ForEach(hook => hook.MessageSent(message));
		}

		public bool Equals(SenderNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.node, node);
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
			return (node != null ? node.GetHashCode() : 0);
		}
	}
}