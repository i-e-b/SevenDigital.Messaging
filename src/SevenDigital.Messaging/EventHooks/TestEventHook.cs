using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SevenDigital.Messaging.EventHooks
{
	/// <summary>
	/// Standard event hook for capturing events
	/// </summary>
	public class TestEventHook : ITestEventHook
	{
		ConcurrentBag<IMessage> sentMessages = new ConcurrentBag<IMessage>();
		ConcurrentBag<IMessage> receivedMessages = new ConcurrentBag<IMessage>();
		ConcurrentBag<Exception> handlerExceptions = new ConcurrentBag<Exception>();

		/// <summary>
		/// A message was sent from this process
		/// </summary>
		/// <param name="message">The message sent</param>
		public void MessageSent(IMessage message)
		{
			sentMessages.Add(message);
		}

		/// <summary>
		/// A message was received and handled by this process
		/// </summary>
		/// <param name="message">The incoming message</param>
		public void MessageReceived(IMessage message)
		{
			receivedMessages.Add(message);
		}

		/// <summary>
		/// A message was received by this process, but a handler threw an exception
		/// </summary>
		/// <param name="message">The incoming message</param>
		/// <param name="handler">Type of the failed handler</param>
		/// <param name="ex">Exception thrown</param>
		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			handlerExceptions.Add(ex);
		}

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