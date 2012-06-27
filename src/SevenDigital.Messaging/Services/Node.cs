using System;
using MassTransit;
using SevenDigital.Messaging.Domain;

namespace SevenDigital.Messaging.Services
{
	public class Node
	{
		readonly IMessagingHost _host;
		readonly Endpoint _endpoint;
		readonly IServiceBusFactory _serviceBusFactory;
		IServiceBus _serviceBus;

		public Node(IMessagingHost host, Endpoint endpoint, IServiceBusFactory serviceBusFactory)
		{
			_host = host;
			_endpoint = endpoint;
			_serviceBusFactory = serviceBusFactory;
		}

		public Uri Address
		{
			get { return new Uri( "rabbitmq://" + _host + "/" + _endpoint); }
		}

		public IServiceBus EnsureConnection()
		{
			return _serviceBus ?? (_serviceBus = _serviceBusFactory.Create(Address));
		}

		public bool Equals(Node other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._host, _host) && Equals(other._endpoint, _endpoint);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Node)) return false;
			return Equals((Node) obj);
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
			if (_serviceBus != null) _serviceBus.Dispose();
		}
	}
}