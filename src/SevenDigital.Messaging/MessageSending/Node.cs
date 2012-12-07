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
		readonly IDispatchController dispatchController;
		IDestinationPoller destinationPoller;
		string endpoint;

		public Node(IMessagingBase messagingBase, IMessageDispatcher messageDispatcher, IDispatchController dispatchController)
		{
			this.messagingBase = messagingBase;
			this.messageDispatcher = messageDispatcher;
			this.dispatchController = dispatchController;
		}

		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint.ToString();
			destinationPoller = dispatchController.CreatePoller(endpoint);
		}

		public void SubscribeHandler<T>(Action<T> action) where T: class
		{
			messagingBase.CreateDestination<T>(endpoint);
			messageDispatcher.AddHandler(action);
			destinationPoller.SetDestinationToWatch(endpoint);
			destinationPoller.Start();
		}

		public void Dispose()
		{
			destinationPoller.Stop();
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