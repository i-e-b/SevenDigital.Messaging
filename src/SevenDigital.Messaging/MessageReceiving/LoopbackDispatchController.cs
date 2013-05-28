namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// No-op dispatcher for loopback
	/// </summary>
	public class LoopbackDispatchController : IDispatchController
	{
		/// <summary>
		/// does nothing
		/// </summary>
		public IDestinationPoller CreatePoller(string destinationName)
		{
			return null;
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public void Shutdown()
		{
		}
	}
}