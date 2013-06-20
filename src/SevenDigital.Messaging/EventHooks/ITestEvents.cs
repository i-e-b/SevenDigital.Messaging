using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging.EventHooks
{
	/// <summary>
	/// Contract for event hooks that store events 
	/// </summary>
	public interface ITestEvents
	{
		/// <summary>
		/// All messages sent since last reset
		/// </summary>
		IEnumerable<IMessage> SentMessages { get; }

		/// <summary>
		/// All messages received since last reset
		/// </summary>
		IEnumerable<IMessage> ReceivedMessages { get; }

		/// <summary>
		/// All exceptions thrown by handlers since last reset
		/// </summary>
		IEnumerable<Exception> HandlerExceptions { get; }

		/// <summary>
		/// Clear all events
		/// </summary>
		void Reset();
	}
}