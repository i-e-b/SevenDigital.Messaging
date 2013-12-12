namespace SevenDigital.Messaging.ConfigurationActions
{
	/// <summary>
	/// Paths for Incoming and Dispatch queues
	/// </summary>
	public class LocalQueueConfig
	{
		/// <summary> Subpath for handler transactions </summary>
		public const string DispatchQueueSubpath = "dispatch";

		/// <summary> Subpath for storing incoming messages </summary>
		public const string IncomingQueueSubpath = "incoming";


		/// <summary>
		/// File path for incoming messages (receive uses this one, pumping messages to the dispatch path)
		/// </summary>
		public string IncomingPath { get; set; }

		/// <summary>
		/// File path for messages being handled (receive uses this one)
		/// </summary>
		public string DispatchPath { get; set; }

		/// <summary>
		/// File path for outgoing messages (send uses this one)
		/// <para>By default, this is the same as <see cref="IncomingPath"/></para>
		/// </summary>
		public string WritePath { get; set; }
	}
}