namespace SevenDigital.Messaging.ConfigurationActions
{
	/// <summary>
	/// Paths for Incoming and Dispatch queues
	/// </summary>
	public class LocalQueueConfig
	{
		/// <summary>
		/// File path for incoming messages (send uses this one)
		/// </summary>
		public string IncomingPath { get; set; }

		/// <summary>
		/// File path for messages being handled (receive uses this one)
		/// </summary>
		public string DispatchPath { get; set; }
	}
}