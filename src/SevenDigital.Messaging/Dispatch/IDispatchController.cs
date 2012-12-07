using System;

namespace SevenDigital.Messaging.Dispatch
{
	public interface IDispatchController : IDisposable
	{
		IDestinationPoller CreatePoller(string destinationName);
		void Shutdown();
	}
}