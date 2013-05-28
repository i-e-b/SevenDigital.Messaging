using System;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Generic action performed by a handler. This maps to `IHandle&lt;T&gt;`
	/// </summary>
	public delegate Exception HandlerAction<T>(T message) where T : class, IMessage;

	/// <summary>
	/// General contract for a receiver node in messaging
	/// </summary>
	public interface INode : IDisposable
	{
		/// <summary>
		/// Set this node to listen on a messaging endpoint.
		/// </summary>
		void SetEndpoint(IRoutingEndpoint endpoint);

		
		/// <summary>
		/// Bind a message type/handler type pair to consume messages on this node
		/// </summary>
		void SubscribeHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;
		
		/// <summary>
		/// Unbind a handler from all messages
		/// </summary>
		void RemoveHandler<T>();
	}
}