namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void AddDestinationToWatch(string destination);
		void Start();
		void Stop();
	}
}