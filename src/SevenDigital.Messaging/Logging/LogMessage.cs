using System;

namespace SevenDigital.Messaging.Logging
{
	public class LogMessage : ILogMessage
	{
		public LogMessage()
		{
			LogDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}
		public string Message { get; set; }
		public string LogDate { get; set; }
	}

	public interface ILogMessage
	{
		string Message { get; set; }
		string LogDate { get; set; }
	}
}