using System.Collections.Generic;
using System.Linq;
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
		Thread pollingThread;
		volatile bool running;

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
			lock (destinations)
			{
				destinations.Add(destination);
			}
		}

		public void PollingMethod()
		{
			object message = null;

			while (running)
			{
				var messageCount = 0;
				var currentDestinations = destinations.ToArray();
				foreach (var destination in currentDestinations)
				{
					if (pool.IsThreadAvailable()) message = messagingBase.GetMessage<IMessage>(destination);
					if (message == null) continue;

					dispatcher.TryDispatch(message);
					messageCount++;
				}

				if (messageCount < 1) sleeper.Sleep();
			}
		}

		public void Start()
		{
			running = true;
			if (pollingThread.ThreadState == ThreadState.Stopped) pollingThread = new Thread(PollingMethod);
			if (pollingThread.ThreadState != ThreadState.Running) pollingThread.Start();
		}

		public void Stop()
		{
			if (running)
			{
				running = false;
				pollingThread.Join();
			}
		}
	}
}