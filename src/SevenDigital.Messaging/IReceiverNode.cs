using System;
using SevenDigital.Messaging.MessageSending;

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
		/// <typeparam name="T">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		IMessageBinding<T> Handle<T>() where T : class, IMessage;

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		string DestinationName { get; }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="T">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		void Unregister<T>();

	}

	/// <summary>
	/// Contract for binding messages to handlers
	/// </summary>
	public interface IBindingHost
	{
		/// <summary>
		/// Bind a message to a handler (non-exclusively)
		/// </summary>
		/// <param name="messageType">Type of incoming message</param>
		/// <param name="handlerType">Handler that should be created and called</param>
		void BindHandler(Type messageType, Type handlerType);
	}
}