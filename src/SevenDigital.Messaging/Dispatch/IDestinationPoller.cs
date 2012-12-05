namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void Start();
		void Stop();
	}
}