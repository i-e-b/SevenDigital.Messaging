using System;

namespace SevenDigital.Messaging.EventHooks
{
	/// <summary>
	/// Event hook that writes events to the console
	/// </summary>
	public class ConsoleEventHook : IEventHook
	{
		/// <summary>
		/// A message was sent from this process
		/// </summary>
		/// <param name="message">The message sent</param>
		public void MessageSent(IMessage message)
		{
			Console.WriteLine("Sent: " + message);
		}

		/// <summary>
		/// A message was received and handled by this process
		/// </summary>
		/// <param name="message">The incoming message</param>
		public void MessageReceived(IMessage message)
		{
			Console.WriteLine("Got: " + message);
		}

		/// <summary>
		/// A message was received by this process, but a handler threw an exception
		/// </summary>
		/// <param name="message">The incoming message</param>
		/// <param name="handler">Type of the failed handler</param>
		/// <param name="ex">Exception thrown</param>
		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			Console.WriteLine("Handler '" + handler.Name + "' failed on message '" + message + "' throwing: " + ex.Message);
		}
	}
}