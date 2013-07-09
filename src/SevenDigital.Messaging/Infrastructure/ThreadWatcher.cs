using System;
using System.Threading;

namespace SevenDigital.Messaging.Infrastructure
{
	/// <summary>
	/// Thread watcher
	/// </summary>
	public class ThreadWatcher
	{
		readonly Thread _threadToWatch;
		readonly Action _onEnd;

		/// <summary>
		/// Watch a thread, and perform an action when it ends
		/// </summary>
		/// <param name="threadToWatch">Thread to watch</param>
		/// <param name="onEnd">Action to perform when thread exits</param>
		public ThreadWatcher(Thread threadToWatch, Action onEnd)
		{
			if ( ! IsValidAndRunning(threadToWatch)) 
				throw new ArgumentException("Thread to watch must be a valid running thread", "threadToWatch");

			if (onEnd == null)
				throw new ArgumentException("Must provide an action to run", "onEnd");

			_threadToWatch = threadToWatch;
			_onEnd = onEnd;

			var tw = new Thread(ThreadLoop);
			tw.Start();
		}

		static bool IsValidAndRunning(Thread threadToWatch)
		{
			return threadToWatch != null && threadToWatch.IsAlive;
		}

		void ThreadLoop()
		{
			_threadToWatch.Join();
			_onEnd();
		}
	}
}