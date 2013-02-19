using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
		void AddHandler<T>(HandlerAction<T> action) where T : class, IMessage;
	}
}