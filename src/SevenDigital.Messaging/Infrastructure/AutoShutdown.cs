using System;
using System.Threading;

namespace SevenDigital.Messaging.Infrastructure
{
	/// <summary>
	/// Shuts down messaging when the activating thread ends
	/// </summary>
	public class AutoShutdown
	{
		static ThreadWatcher watcher;

		/// <summary>
		/// Shut down messaging when the activating thread ends
		/// </summary>
		public static void Activate()
		{
			watcher = new ThreadWatcher(Thread.CurrentThread, ShutdownAction);
		}

		static void ShutdownAction()
		{
			MessagingSystem.Control.Shutdown();
			GC.KeepAlive(watcher);
		}
	}
}