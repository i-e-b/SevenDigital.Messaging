namespace SevenDigital.Messaging.Dispatch
{
	/// <summary>
	/// Dispatch controller is a simple list
	/// of Destination Pollers with a shut-down switch.
	/// </summary>
	public interface IDispatchController
	{
		IDestinationPoller CreatePoller(string destinationName);
		void Shutdown();
	}
}