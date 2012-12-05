namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void AddDestinationToWatch(string destinationName);
		void Start();
		void Stop();
	}
}