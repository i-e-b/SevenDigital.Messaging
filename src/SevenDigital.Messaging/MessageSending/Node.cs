using System;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class Node: IDisposable
	{
		readonly IMessagingHost _host;
		readonly IRoutingEndpoint _endpoint;
		readonly IDispatchInterface dispatchInterface;

		public Node(IMessagingHost host, IRoutingEndpoint endpoint, IDispatchInterface dispatchInterface)
		{
			_host = host;
			_endpoint = endpoint;
			this.dispatchInterface = dispatchInterface;
		}

		public Uri Address
		{
			get { return new Uri( "rabbitmq://" + _host + "/" + _endpoint); }
		}

		public void Dispose()
		{
			if (dispatchInterface != null) dispatchInterface.Dispose();
		}

		#region Equality members

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

		#endregion
	}
}