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
			Console.WriteLine("Activating shutdown protection");
			watcher = new ThreadWatcher(Thread.CurrentThread, ShutdownAction);
		}

		static void ShutdownAction()
		{
			Console.WriteLine("Auto shutdown...");
			MessagingSystem.Control.Shutdown();
			Console.WriteLine("...done");
			GC.KeepAlive(watcher);
		}
	}
}