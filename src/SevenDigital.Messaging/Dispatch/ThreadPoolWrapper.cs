using System;
using System.Threading;

namespace SevenDigital.Messaging.Dispatch
{
	public class ThreadPoolWrapper : IThreadPoolWrapper
	{
		public void Do(Action<object> action)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(action));
		}

		public bool IsThreadAvailable()
		{
			int workers, completion;
			ThreadPool.GetAvailableThreads(out workers, out completion);

			return (workers > 0);
		}
	}

	public interface IThreadPoolWrapper
	{
		void Do(Action<object> action);
		bool IsThreadAvailable();
	}
}
