using System;
using System.Threading;
using SignalHandling;

namespace SevenDigital.Messaging
{
	public class Cooldown
	{
		static int calls;

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
			Messaging.Control.Shutdown();
			Console.WriteLine("All handlers done. Exiting");
			Environment.Exit(0);
		}
	}
}
