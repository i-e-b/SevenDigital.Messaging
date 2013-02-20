using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
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