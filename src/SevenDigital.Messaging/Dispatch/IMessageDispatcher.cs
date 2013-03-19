using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
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
		void TryDispatch(IPendingMessage<object> pendingMessage);
		int HandlersInflight { get; }
		void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		void RemoveHandler<T>();
		int CountHandlers();
	}
}