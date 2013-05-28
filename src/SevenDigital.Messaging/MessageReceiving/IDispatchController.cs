namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Dispatch controller is a simple list
	/// of Destination Pollers with a shut-down switch.
	/// </summary>
	public interface IDispatchController
	{
		/// <summary>
		/// Create a poller for a given destination name
		/// </summary>
		IDestinationPoller CreatePoller(string destinationName);

		/// <summary>
		/// Stop polling
		/// </summary>
		void Shutdown();
	}
}