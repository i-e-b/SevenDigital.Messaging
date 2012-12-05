using System.Collections.Generic;
using System.Threading;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class DestinationPoller : IDestinationPoller
	{
		readonly ISet<string> destinations;
		readonly IMessagingBase messagingBase;
		readonly ISleepWrapper sleeper;
		readonly IDispatcher dispatcher;
		readonly IThreadPoolWrapper pool;
		readonly Thread pollingThread;

		public DestinationPoller(IMessagingBase messagingBase, ISleepWrapper sleeper, IDispatcher dispatcher, IThreadPoolWrapper pool)
		{
			this.messagingBase = messagingBase;
			this.sleeper = sleeper;
			this.dispatcher = dispatcher;
			this.pool = pool;

			destinations = new HashSet<string>();
			pollingThread = new Thread(PollingMethod);
		}

		public void AddDestinationToWatch(string destination)
		{
			destinations.Add(destination);
		}

		public void PollingMethod()
		{
			var messageCount = 0;
			object message = null;

			foreach (var destination in destinations)
			{
				if (pool.IsThreadAvailable()) message = messagingBase.GetMessage<IMessage>(destination);
				if (message == null) continue;

				dispatcher.TryDispatch(message);
				messageCount++;
			}

			if (messageCount < 1) sleeper.Sleep();
		}

		public void Start()
		{
			pollingThread.Start();
		}

		public void Stop()
		{

		}
	}
}