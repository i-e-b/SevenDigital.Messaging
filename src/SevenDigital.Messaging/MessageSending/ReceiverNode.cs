using System;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard receiver node for Messaging.
	/// You do not need to create this yourself. Use `Messaging.Receiver()`
	/// </summary>
	public class ReceiverNode : IReceiverNode
	{
		readonly IRoutingEndpoint endpoint;
		readonly INode node;

		/// <summary>
		/// Create a new message receiver node. You do not need to create this yourself. Use `Messaging.Receiver()`
		/// </summary>
		public ReceiverNode(IRoutingEndpoint endpoint)
		{
			this.endpoint = endpoint;
			node = ObjectFactory.GetInstance<INode>();
			node.SetEndpoint(endpoint);
		}

		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="T">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			if (!typeof(T).IsInterface) throw new ArgumentException("Handler type must be an interface that implements IMessage");
			return new HandlerTriggering<T>(node);
		}

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return endpoint.ToString(); } }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="T">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		public void Unregister<T>()
		{
			node.RemoveHandler<T>();
		}

		#region Equality members

#pragma warning disable 1591
		public bool Equals(ReceiverNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.endpoint, endpoint);
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
#pragma warning restore 1591

		#endregion
	}
}