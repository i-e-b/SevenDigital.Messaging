using System;

namespace SevenDigital.Messaging.Logging
{
	/// <summary>
	/// Logging event message.
	/// </summary>
	public class MessagingLogEventArgs : EventArgs
	{
		/// <summary>
		/// Create a new log message
		/// </summary>
		public MessagingLogEventArgs(string message)
		{
			LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			Message = message;
		}

		/// <summary>
		/// Message to log
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Date logged
		/// </summary>
		public string LogDate { get; set; }
	}
}