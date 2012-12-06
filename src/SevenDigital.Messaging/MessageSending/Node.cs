using System;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class Node: INode
	{
		readonly IMessagingBase messagingBase;
		readonly IMessageDispatcher messageDispatcher;
		readonly IDestinationPoller destinationPoller;
		string endpoint;

		public Node(IMessagingBase messagingBase, IMessageDispatcher messageDispatcher, IDestinationPoller destinationPoller)
		{
			this.messagingBase = messagingBase;
			this.messageDispatcher = messageDispatcher;
			this.destinationPoller = destinationPoller;
		}

		public void Dispose()
		{
			destinationPoller.Stop();
		}

		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint.ToString();
		}

		public void SubscribeHandler<T>(Action<T> action) where T: class
		{
			messagingBase.CreateDestination<T>(endpoint);
			messageDispatcher.AddHandler(action);
			destinationPoller.SetDestinationToWatch(endpoint);
			destinationPoller.Start();
		}

		#region Equality members

		public bool Equals(Node other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.endpoint, endpoint);
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
				return (endpoint != null ? endpoint.GetHashCode() : 0);
			}
		}

		#endregion
	}
}