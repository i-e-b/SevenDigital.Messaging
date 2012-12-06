namespace SevenDigital.Messaging.Dispatch
{
	public interface IMessageToHandlerDispatcher
	{
		void TryDispatch(object fakeMsg);
	}
}