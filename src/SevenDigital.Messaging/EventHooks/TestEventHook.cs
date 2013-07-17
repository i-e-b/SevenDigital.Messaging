using System;

namespace SevenDigital.Messaging.EventHooks
{
	/// <summary>
	/// Standard event hook for capturing events
	/// </summary>
	public class TestEventHook : IEventHook
	{
		readonly ITestEvents _testEvents;
		readonly object _lock = new object();

		/// <summary>
		/// Creates a test event hook. Don't do this yourself -- use
		/// `MessagingSystem.Configure.WithLoopbackMode()`
		/// </summary>
		public TestEventHook(ITestEvents testEvents)
		{
			_testEvents = testEvents;
		}

		/// <summary>
		/// A message was sent from this process
		/// </summary>
		/// <param name="message">The message sent</param>
		public void MessageSent(IMessage message)
		{
			lock(_lock)
			{
				((TestEvents) _testEvents).sentMessages.Add(message);
			}
		}

		/// <summary>
		/// A message was received and handled by this process
		/// </summary>
		/// <param name="message">The incoming message</param>
		public void MessageReceived(IMessage message)
		{
			lock(_lock)
			{
				((TestEvents) _testEvents).receivedMessages.Add(message);
			}
		}

		/// <summary>
		/// A message was received by this process, but a handler threw an exception
		/// </summary>
		/// <param name="message">The incoming message</param>
		/// <param name="handler">Type of the failed handler</param>
		/// <param name="ex">Exception thrown</param>
		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			lock(_lock)
			{
				((TestEvents) _testEvents).handlerExceptions.Add(ex);
			}
		}
	}
}