using System;
using System.Threading;
using SignalHandling;

namespace SevenDigital.Messaging
{
	public class Cooldown
	{
		static int calls = 0;

		public static void Activate()
		{
			new Thread(() =>
			{
				try
				{
					if (Interlocked.CompareExchange(ref calls, 1, 0) == 0)
					{
						Console.WriteLine("Activating cooldown capture");
						CrossPlatformSignalDispatch.Instance.TerminateEvent += Instance_TerminateEvent;
					}
				}
				catch (System.DllNotFoundException ex)
				{
					Console.WriteLine("Failed to start cooldown - " + ex.GetType() + ": " + ex.Message);
				}
			}).Start();
		}

		static void Instance_TerminateEvent(object sender, TerminateEventArgs args)
		{
			Console.WriteLine("Waiting for all handlers to exit");
			new MessagingConfiguration().Shutdown();
			Console.WriteLine("All handlers done. Exiting");
			Environment.Exit(0);
		}
	}
}
