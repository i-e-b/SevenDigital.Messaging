namespace SevenDigital.Messaging.Logging
{
	public class LogMessage : ILogMessage
	{
		public string Message { get; set; }
	}

	public interface ILogMessage
	{
		string Message { get; set; }
	}
}