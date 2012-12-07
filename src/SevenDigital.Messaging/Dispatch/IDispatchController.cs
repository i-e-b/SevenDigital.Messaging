namespace SevenDigital.Messaging.Dispatch
{
	public interface IDispatchController
	{
		IDestinationPoller CreatePoller(string destinationName);
		void Shutdown();
	}
}