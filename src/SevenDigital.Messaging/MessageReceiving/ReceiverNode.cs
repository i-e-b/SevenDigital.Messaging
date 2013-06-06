using System;
using DispatchSharp;
using DispatchSharp.WorkerPools;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageReceiving
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
		readonly IReceiverControl _parent;
		readonly IRoutingEndpoint _endpoint;
		readonly IDispatch<IPendingMessage<object>> _receivingDispatcher;
		readonly IHandlerManager _handler; // message type => [handler types]
		readonly ITypedPollingNode _pollingNode;

		/// <summary>
		/// Create a new message receiver node. You do not need to create this yourself. Use `Messaging.Receiver()`
		/// </summary>
		public ReceiverNode(
			IReceiverControl parent,
			IRoutingEndpoint endpoint,
			IHandlerManager handler,
			IPollingNodeFactory pollerFactory,
			IDispatcherFactory dispatchFactory)
		{
			_parent = parent;
			_endpoint = endpoint;
			_handler = handler;
			_pollingNode = pollerFactory.Create(endpoint);

			_receivingDispatcher = dispatchFactory.Create(
				_pollingNode,
				new ThreadedWorkerPool<IPendingMessage<object>>("SDMessaging_Receiver")
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
		/// Set maximum number of concurrent handlers on this node
		/// </summary>
		public void SetConcurrentHandlers(int max)
		{
			_receivingDispatcher.SetMaximumInflight(max);
		}

		/// <summary>
		/// Bind a message to a handler (non-exclusively)
		/// </summary>
		/// <param name="messageType">Type of incoming message</param>
		/// <param name="handlerType">Handler that should be created and called</param>
		public void BindHandler(Type messageType, Type handlerType)
		{
			_pollingNode.AddMessageType(messageType);
			_handler.AddHandler(messageType, handlerType);
		}

		/// <summary>
		/// Handles messages received from messaging base
		/// </summary>
		public void HandleIncomingMessage(IPendingMessage<object> incoming)
		{
			_handler.TryHandle(incoming);
		}

		/// <summary>
		/// Stop this node and deregister from parent.
		/// </summary>
		public void Dispose()
		{
			_receivingDispatcher.Stop();
			_parent.Remove(this);
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

		#region Message to Handler binding API
		class MessageBinder<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
		{
			readonly IBindingHost _host;

			/// <summary>
			/// create a handler binding api point
			/// </summary>
			public MessageBinder(IBindingHost host)
			{
				_host = host;
			}

			/// <summary>
			/// Bind this handler to receive the selected message type.
			/// The handler may receive any number of messages immediately after calling this method
			/// until unbound or messaging is paused or shutdown.
			/// </summary>
			public void With<THandler>() where THandler : IHandle<TMessage>
			{
				_host.BindHandler(typeof(TMessage), typeof(THandler));
			}
		}
		#endregion
	}
}