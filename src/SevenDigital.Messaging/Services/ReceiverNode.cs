using SevenDigital.Messaging.Domain;

namespace SevenDigital.Messaging.Services
{
	public class ReceiverNode : IReceiverNode
	{
		readonly IMessagingHost _host;
		readonly Endpoint _endpoint;
		readonly IServiceBusFactory _serviceBusFactory;
		readonly Node _node;

		public ReceiverNode(IMessagingHost host, Endpoint endpoint, IServiceBusFactory serviceBusFactory)
		{
			_host = host;
			_endpoint = endpoint;
			_serviceBusFactory = serviceBusFactory;
			_node = new Node(host, endpoint, _serviceBusFactory);
		}

		public bool Equals(ReceiverNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._host, _host) && Equals(other._endpoint, _endpoint);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ReceiverNode)) return false;
			return Equals((ReceiverNode) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_host != null ? _host.GetHashCode() : 0)*397) ^ (_endpoint != null ? _endpoint.GetHashCode() : 0);
			}
		}

		public void Dispose()
		{
			_node.Dispose();
		}

		public MessageBinding<T> Handle<T>() where T : class, IMessage
		{
			var serviceBus = _node.EnsureConnection();
			return new MessageBinding<T>(serviceBus);
		}
	}
}