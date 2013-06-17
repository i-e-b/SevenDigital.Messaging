using System;
using System.Threading;

namespace SevenDigital.Messaging.Logging
{
	/// <summary>
	/// Messaging based logger
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Event fired when messaging wishes to log
		/// </summary>
		public event EventHandler<MessagingLogEventArgs> LogEvent;

		/// <summary>
		/// Fire a log event
		/// </summary>
		public void OnLogEvent(MessagingLogEventArgs e)
		{
			var handler = LogEvent;
			if (handler != null) handler(this, e);
		}

		static Log instance;
		readonly object _lock = new object();

		/// <summary>
		/// Active instance of log
		/// </summary>
		public static Log Instance()
		{
			Interlocked.CompareExchange(ref instance, new Log(), null);
			return instance;
		}

		/// <summary>
		/// Log a message to the warning queue
		/// </summary>
		public static void Warning(string message)
		{
			Instance().OnLogEvent(new MessagingLogEventArgs(message));
		}

		/// <summary>
		/// Register an action for the log event
		/// </summary>
		public void RegisterAction(Action<MessagingLogEventArgs> action)
		{
			lock (_lock)
			{
				LogEvent += (s,e) => action(e);
			}
		}

		/// <summary>
		/// Remove all registered actions
		/// </summary>
		public void Shutdown()
		{
			lock (_lock)
			{
				var handler = LogEvent;
				if (handler == null) return;
				foreach (var del in handler.GetInvocationList())
				{
					LogEvent -= (EventHandler<MessagingLogEventArgs>)del;
				}
			}
		}
	}
}