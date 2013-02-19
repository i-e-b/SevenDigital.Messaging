using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageDispatcher
	{
		void TryDispatch(IPendingMessage<object> pendingMessage);
		void AddHandler<T>(HandlerAction<T> handlerAction) where T : class, IMessage;
		int HandlersInflight { get; }
	}
}