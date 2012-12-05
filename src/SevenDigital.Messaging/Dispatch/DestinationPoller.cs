using System.Threading;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.Dispatch
{
	public class DestinationPoller : IDestinationPoller
	{
		readonly string destination;
		readonly IMessagingBase messagingBase;
		readonly ISleepWrapper sleeper;
		readonly IDispatcher dispatcher;
		readonly IThreadPoolWrapper pool;
		readonly Thread pollingThread;

		public DestinationPoller(string destination, IMessagingBase messagingBase, ISleepWrapper sleeper, IDispatcher dispatcher, IThreadPoolWrapper pool)
		{
			this.destination = destination;
			this.messagingBase = messagingBase;
			this.sleeper = sleeper;
			this.dispatcher = dispatcher;
			this.pool = pool;
			pollingThread = new Thread(PollingMethod);
		}

		public void PollingMethod()
		{
			object message = null;
			if (pool.IsThreadAvailable())
			{
				message = messagingBase.GetMessage<IMessage>(destination);
			}
			if (message != null)
			{
				dispatcher.TryDispatch(message);
			}
			else
			{
				sleeper.Sleep();
			}
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