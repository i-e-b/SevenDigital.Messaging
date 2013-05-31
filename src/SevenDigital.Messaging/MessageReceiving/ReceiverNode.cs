using System;
using DispatchSharp;
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
	/// <remarks>
	/// The receiver node binds a rabbit work queue to a message handler.
	/// When the user binds a message type to a handler, this gets added to the
	/// handler and the rabbit work queue.
	/// </remarks>
	public class ReceiverNode : IReceiverNode, IBindingHost
	{
		readonly IRoutingEndpoint _endpoint;
		readonly IDispatch<IPendingMessage<object>> _receivingDispatcher;
		readonly IMessageHandler _handler; // message type => [handler types]
		readonly RabbitMqPollingNode _rabbitMqPollingNode;

		/// <summary>
		/// Create a new message receiver node. You do not need to create this yourself. Use `Messaging.Receiver()`
		/// </summary>
		public ReceiverNode(
			IRoutingEndpoint endpoint,
			IMessageHandler handler,
			IMessagingBase messagingBase,
			ISleepWrapper sleeper)
		{
			_endpoint = endpoint;
			_handler = handler;
			_rabbitMqPollingNode = new RabbitMqPollingNode(endpoint, messagingBase, sleeper);

			_receivingDispatcher = new Dispatch<IPendingMessage<object>>( 
				_rabbitMqPollingNode,
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
		public string DestinationName { get { return _endpoint.ToString(); } }

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
			_rabbitMqPollingNode.AddMessageType(messageType);
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
			return Equals(other._endpoint, _endpoint);
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
				return (_endpoint != null ? _endpoint.GetHashCode() : 0);
			}
		}
#pragma warning restore 1591

		#endregion
	}
}