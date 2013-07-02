using System;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// A messaging node that can receive messages and pass them to handlers
	/// </summary>
	public interface IReceiverNode : IDisposable
	{
		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="TMessage">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		[Obsolete("This configuration method has a race condition. Please use `Register(b=>b.Handle<message>().With<>())` instead")]
		IHandlerBinding<TMessage> Handle<TMessage>() where TMessage : class, IMessage;

		/// <summary>
		/// Bind messages to handler types.
		/// </summary>
		void Register(params Action<IMessageBinding>[] bindings);

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		string DestinationName { get; }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="THandler">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		void Unregister<THandler>();

		/// <summary>
		/// Set maximum number of concurrent handlers on this node
		/// </summary>
		void SetConcurrentHandlers(int max);
	}

	/// <summary>
	/// Bind messages to handlers
	/// </summary>
	public interface IMessageBinding
	{
		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="TMessage">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		IHandlerBinding<TMessage> Handle<TMessage>() where TMessage : class, IMessage;
	}

	/// <summary>
	/// A message binder, used to bind handlers to message types in a receiver node
	/// </summary>
	/// <typeparam name="TMessage">Message type to be bound</typeparam>
	public interface IHandlerBinding<TMessage> where TMessage : class, IMessage
	{
		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		void With<THandler>() where THandler : IHandle<TMessage>;
	}

	/// <summary>
	/// Contract for binding messages to handlers
	/// </summary>
	public interface IBindingHost
	{
		/// <summary>
		/// Bind a message to a handler (non-exclusively)
		/// </summary>
		void BindHandlers(Tuple<Type, Type>[] messageType_handlerType);
		
		/// <summary>
		/// Bind a message to a handler (non-exclusively)
		/// </summary>
		void BindHandler(Type messageType, Type handlerType);
	}
}