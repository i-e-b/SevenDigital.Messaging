using System;

namespace SevenDigital.Messaging.Logging
{
	/// <summary>
	/// Log message concrete
	/// </summary>
	public class LogMessage : ILogMessage
	{
		/// <summary>
		/// Create a new log message
		/// </summary>
		public LogMessage()
		{
			LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

	/// <summary>
	/// Log message contract
	/// </summary>
	public interface ILogMessage
	{
		/// <summary>
		/// Message to log
		/// </summary>
		string Message { get; set; }
		/// <summary>
		/// Date logged
		/// </summary>
		string LogDate { get; set; }
	}
}