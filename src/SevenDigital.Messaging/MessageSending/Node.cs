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

		protected bool Equals(Node other)
		{
			return Equals(messagingBase, other.messagingBase) && Equals(messageDispatcher, other.messageDispatcher) && Equals(destinationPoller, other.destinationPoller);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Node) obj);
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (messagingBase != null ? messagingBase.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (messageDispatcher != null ? messageDispatcher.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (destinationPoller != null ? destinationPoller.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(Node left, Node right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Node left, Node right)
		{
			return !Equals(left, right);
		}
		#endregion
	}
}