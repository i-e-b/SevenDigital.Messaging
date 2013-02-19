using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
		void AddHandler<T>(Type handlerType) where T : class, IMessage;
	}
}