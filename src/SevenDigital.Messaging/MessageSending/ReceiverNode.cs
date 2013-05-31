using System;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using DispatchSharp.WorkerPools;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard receiver node for Messaging.
	/// You do not need to create this yourself. Use `Messaging.Receiver()`
	/// </summary>
	public class ReceiverNode : IReceiverNode, IBindingHost
	{
		readonly IRoutingEndpoint endpoint;
		readonly IDispatch<IPendingMessage<object>> _receivingDispatcher;
		readonly IMessageHandler _handler; // message type => [handler types]

		/// <summary>
		/// Create a new message receiver node. You do not need to create this yourself. Use `Messaging.Receiver()`
		/// </summary>
		public ReceiverNode(IRoutingEndpoint endpoint, IMessageHandler handler)
		{
			this.endpoint = endpoint;
			_handler = handler;

			_receivingDispatcher = new Dispatch<IPendingMessage<object>>( 
				new InMemoryWorkQueue<IPendingMessage<object>>(), // should be RMQ polling queue
				new ThreadedWorkerPool<IPendingMessage<object>>("SDMessaging_Receiver", 1)
				);

			_receivingDispatcher.AddConsumer(HandleIncomingMessage);
			_receivingDispatcher.Start();
		}

		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="T">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			if (!typeof(T).IsInterface) throw new ArgumentException("Handler type must be an interface that implements IMessage");
			return new MessageBinder<T>(this);
		}

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return endpoint.ToString(); } }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="THandler">Type of hander previously bound with `Handle&lt;T&gt;().With&lt;THandler&gt;()`</typeparam>
		public void Unregister<THandler>()
		{
			_handler.RemoveHandler(typeof(THandler));
		}

		/// <summary>
		/// Bind a message to a handler (non-exclusively)
		/// </summary>
		/// <param name="messageType">Type of incoming message</param>
		/// <param name="handlerType">Handler that should be created and called</param>
		public void BindHandler(Type messageType, Type handlerType)
		{
			_handler.AddHandler(messageType, handlerType);
		}

		void HandleIncomingMessage(IPendingMessage<object> incoming)
		{
			_handler.TryHandle(incoming);
		}

		public void Dispose()
		{
			_receivingDispatcher.Stop();
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
#pragma warning restore 1591

		#endregion
	}
}