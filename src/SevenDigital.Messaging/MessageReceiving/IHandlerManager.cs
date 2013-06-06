using System;
using System.Collections.Generic;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Message dispatcher keeps a set of bindings 
	/// between message types and handler types.
	/// 
	/// TryDispatch starts a handler instance and passes it
	/// an incoming message.
	/// </summary>
	public interface IHandlerManager
	{
		/// <summary>
		/// Try to fire actions for a message
		/// </summary>
		void TryHandle(IPendingMessage<object> pendingMessage);

		/// <summary>
		/// Add a handler/message binding
		/// </summary>
		void AddHandler(Type messageType, Type handlerType);
			//where TMessage : class, IMessage
			//where THandler : IHandle<TMessage>;

		/// <summary>
		/// remove a handler for all messages
		/// </summary>
		void RemoveHandler(Type handlerType);

		/// <summary>
		/// Return count of handlers
		/// </summary>
		int CountHandlers();

		/// <summary>
		/// List handlers that could process a given message type.
		/// More generic handlers will be returned for more specific message types
		/// </summary>
		IEnumerable<Type> GetMatchingHandlers(Type type);
	}
}