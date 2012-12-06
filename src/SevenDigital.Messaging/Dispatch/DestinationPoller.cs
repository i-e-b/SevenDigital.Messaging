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
		readonly IMessageDispatcher dispatcher;
		readonly IThreadPoolWrapper pool;
		Thread pollingThread;
		volatile bool running;

		public DestinationPoller(IMessagingBase messagingBase, ISleepWrapper sleeper, IMessageDispatcher dispatcher, IThreadPoolWrapper pool)
		{
			this.messagingBase = messagingBase;
			this.sleeper = sleeper;
			this.dispatcher = dispatcher;
			this.pool = pool;

			destinations = new HashSet<string>();
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
			while (running)
			{
				var messageCount = 0;
				var currentDestinations = destinations.ToArray();
				foreach (var destination in currentDestinations)
				{
					object message = null;
					if (pool.IsThreadAvailable()) message = GetMessageRobust(destination);
					if (message == null) continue;

					dispatcher.TryDispatch(message);
					messageCount++;
				}

				if (messageCount < 1) sleeper.Sleep();
			}
		}

		IMessage GetMessageRobust(string destination)
		{
			// Should be able to re-bind queues here?
			return messagingBase.GetMessage<IMessage>(destination);
		}

		public void Start()
		{
			if (running) return;
			lock (this)
			{
				running = true;
				if (pollingThread == null) pollingThread = new Thread(PollingMethod);
				if (pollingThread.ThreadState != ThreadState.Running) pollingThread.Start();
			}
		}

		public void Stop()
		{
			var pt = pollingThread;
			running = false;
			if (pt != null) pt.Join();
			pollingThread = null;
		}
	}
}