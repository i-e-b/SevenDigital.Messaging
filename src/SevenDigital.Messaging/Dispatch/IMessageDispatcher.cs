using System;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageDispatcher
	{
		void TryDispatch(object messageObject);
		void AddHandler<T>(HandlerAction<T> handlerAction) where T : class, IMessage;
		int HandlersInflight { get; }
	}
}