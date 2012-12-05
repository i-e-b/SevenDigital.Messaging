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
	}

	public interface IThreadPoolWrapper
	{
		void Do(Action<object> action);
	}
}
