using System;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatcher : IMessageDispatcher
	{
		public void TryDispatch(object fakeMsg)
		{
		}

		public void AddHandler<T>(Action<T> handlerAction)
		{
		}
	}
}