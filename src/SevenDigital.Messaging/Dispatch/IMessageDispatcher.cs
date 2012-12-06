using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageDispatcher
	{
		void TryDispatch(object fakeMsg);
		void AddHandler<T>(Action<T> handlerAction);
	}
}