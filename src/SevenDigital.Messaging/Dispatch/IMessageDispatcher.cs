using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageDispatcher
	{
		void TryDispatch(object messageObject);
		void AddHandler<T>(Action<T> handlerAction);
	}
}