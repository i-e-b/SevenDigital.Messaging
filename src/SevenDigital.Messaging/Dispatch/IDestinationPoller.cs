using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
		void AddHandler<T>(Action<T> action);
	}
}