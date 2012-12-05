using System.Collections.Generic;
using System.Threading;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class DestinationPoller : IDestinationPoller
	{
		readonly IMessagingBase messagingBase;
		readonly Thread pollingThread;
		readonly ISet<string> destinations;

		public DestinationPoller(IMessagingBase messagingBase)
		{
			this.messagingBase = messagingBase;
			pollingThread = new Thread(PollingMethod){IsBackground = true};
			pollingThread.Start();
			destinations = new HashSet<string>();
		}

		public void AddDestinationToWatch(string destinationName)
		{
			destinations.Add(destinationName);
		}

		public void PollingMethod()
		{
			foreach (var destination in destinations)
			{
				messagingBase.GetMessage<IMessage>(destination);
			}
		}

		public void Start()
		{
		}

		public void Stop()
		{

		}
	}
}