using System;
using System.Threading;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class DestinationPoller : IDestinationPoller
	{
		string destination;
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
		}

		public void SetDestinationToWatch(string targetDestination)
		{
			destination = targetDestination;
		}

		public void PollingMethod()
		{
			var sleep = 0;
			while (running)
			{
				object message = null;
				if (pool.IsThreadAvailable()) message = GetMessageRobust();
				if (message != null)
				{
					dispatcher.TryDispatch(message);
					sleep = 0;
				}
				else
				{
					sleeper.Sleep(sleep);
					sleep = burstSleep(sleep);
				}
			}
		}

		int burstSleep(int sleep)
		{
			switch (sleep)
			{
				case 0: return 1;
				case 1: return 50;
				default: return 250;
			}
		}

		IMessage GetMessageRobust()
		{
			try
			{
				return messagingBase.GetMessage<IMessage>(destination);
			}
			catch (Exception)
			{
				return null;
			}
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
			pollingThread = null;
			running = false;
			if (pt == null) return;

			pt.IsBackground = true;
		}

		public void AddHandler<T>(Action<T> action)
		{
			dispatcher.AddHandler(action);
		}
	}
}