using System;
using System.Threading;
using SignalHandling;

namespace SevenDigital.Messaging.Infrastructure
{
	/// <summary>
	/// Utility for setting up the termination cooldown event handler
	/// </summary>
	public class Cooldown
	{
		static int calls;

		/// <summary>
		/// Activate cooldown capture for this process
		/// </summary>
		public static void Activate()
		{
			if (Interlocked.CompareExchange(ref calls, 1, 0) == 0)
			{
				CrossPlatformSignalDispatch.Instance.TerminateEvent += Instance_TerminateEvent;
			}
		}

		static void Instance_TerminateEvent(object sender, TerminateEventArgs args)
		{
			Console.WriteLine("Waiting for all handlers to exit");
			MessagingSystem.Control.Shutdown();
			Console.WriteLine("All handlers done. Exiting");
			Environment.Exit(0);
		}
	}
}
