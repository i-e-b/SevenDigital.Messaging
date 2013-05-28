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
	public interface IMessageDispatcher
	{
		/// <summary>
		/// Try to fire actions for a message
		/// </summary>
		void TryDispatch(IPendingMessage<object> pendingMessage);

		/// <summary>
		/// number of handlers currently running and handling messages
		/// </summary>
		int HandlersInflight { get; }

		/// <summary>
		/// Add a handler/message binding
		/// </summary>
		void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		/// <summary>
		/// remove a handler for all messages
		/// </summary>
		void RemoveHandler<T>();

		/// <summary>
		/// Return count of handlers
		/// </summary>
		int CountHandlers();
	}
}