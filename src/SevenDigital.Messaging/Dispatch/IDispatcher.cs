namespace SevenDigital.Messaging.Dispatch
{
	public interface IDispatcher
	{
		void TryDispatch(object fakeMsg);
	}
}