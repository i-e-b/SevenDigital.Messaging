using System;
using System.Threading;
using SevenDigital.Messaging.Infrastructure.SignalHandling.PrivateMonoDerived;

namespace SevenDigital.Messaging.Infrastructure.SignalHandling
{
	/// <summary>
	/// Wait for termination events on a background thread.
	/// You should terminate soon after receiving a terminate event.
	/// </summary>
	public class CrossPlatformSignalDispatch
	{
		/// <summary>
		/// Event triggered when SIGTERM or ^C occur
		/// </summary>
		public event TerminateEvent TerminateEvent;

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static CrossPlatformSignalDispatch Instance
		{
			get
			{
				return instance ?? (instance = new CrossPlatformSignalDispatch());
			}
		}

		static CrossPlatformSignalDispatch instance;
		static bool RunningUnderPosix
		{
			get
			{
				var p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
		void TerminateEventSent(int signal)
		{
			var handler = TerminateEvent;
			if (handler != null)
			{
				handler(this, new TerminateEventArgs(signal));
			}
			else
			{
				Environment.Exit(signal);
			}
		}

		private CrossPlatformSignalDispatch()
		{
			if (RunningUnderPosix)
			{
				waitingThread = new Thread(UnixSignalLoop);
				waitingThread.IsBackground = true;
				waitingThread.Start();
			}
			else
			{
				Console.CancelKeyPress += ConsoleCancelKeyPress;
			}
		}

		void UnixSignalLoop()
		{
			var signals = new[]{
							new UnixSignal (Signum.SIGINT),  // ^C
							new UnixSignal (Signum.SIGTERM), // kill
							new UnixSignal (Signum.SIGHUP)   // background and drop
						};
			while (waitingThread.IsAlive)
			{
				var which = UnixSignal.WaitAny(signals, -1);
				TerminateEventSent((int)signals[which].Signum);
			}
		}

		void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel = true;
			TerminateEventSent((int)Signum.SIGINT);
		}

		readonly Thread waitingThread;
	}

	/// <summary>
	/// Terminate signal event
	/// </summary>
	public delegate void TerminateEvent(object sender, TerminateEventArgs args);

	/// <summary>
	/// Terminate signal event arguments
	/// </summary>
	public class TerminateEventArgs : EventArgs
	{
		/// <summary>
		/// UNIX signal number or equivalent
		/// </summary>
		public int Signal { get; set; }

		/// <summary>
		/// Create new event args with a given signal number
		/// </summary>
		public TerminateEventArgs(int signal)
		{
			Signal = signal;
		}
	}
}
