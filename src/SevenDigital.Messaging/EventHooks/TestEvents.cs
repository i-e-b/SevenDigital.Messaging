using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SevenDigital.Messaging.EventHooks
{
	/// <summary>
	/// Events that happened during a loopback session.
	/// </summary>
	public class TestEvents : ITestEvents
	{
		internal ConcurrentBag<IMessage> sentMessages = new ConcurrentBag<IMessage>();
		internal ConcurrentBag<IMessage> receivedMessages = new ConcurrentBag<IMessage>();
		internal ConcurrentBag<Exception> handlerExceptions = new ConcurrentBag<Exception>();
		
		/// <summary>
		/// All messages sent since last reset
		/// </summary>
		public IEnumerable<IMessage> SentMessages { get { return sentMessages; } }

		/// <summary>
		/// All messages received since last reset
		/// </summary>
		public IEnumerable<IMessage> ReceivedMessages { get { return receivedMessages; } }

		/// <summary>
		/// All exceptions thrown by handlers since last reset
		/// </summary>
		public IEnumerable<Exception> HandlerExceptions { get { return handlerExceptions; } }

		/// <summary>
		/// Clear all events
		/// </summary>
		public void Reset()
		{
			sentMessages = new ConcurrentBag<IMessage>();
			receivedMessages = new ConcurrentBag<IMessage>();
			handlerExceptions = new ConcurrentBag<Exception>();
		}
	}
}