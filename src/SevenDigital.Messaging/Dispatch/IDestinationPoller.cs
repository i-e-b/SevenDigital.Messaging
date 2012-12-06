namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
	}
}