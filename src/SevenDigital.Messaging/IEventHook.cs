using System;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Contract for event hooks.
	/// All bound event hooks are created and called whenever a message is send or received by the current process.
	/// If a handler throws an exception in this process, all event hooks will be notified.
	/// 
	/// Events that happen in other processes will not trigger the hooks in this process.
	/// </summary>
	public interface IEventHook
	{
		/// <summary>
		/// A message was sent from this process
		/// </summary>
		/// <param name="message">The message sent</param>
		void MessageSent(IMessage message);

		/// <summary>
		/// A message was received and handled by this process
		/// </summary>
		/// <param name="message">The incoming message</param>
		void MessageReceived(IMessage message);

		/// <summary>
		/// A message was received by this process, but a handler threw an exception
		/// </summary>
		/// <param name="message">The incoming message</param>
		/// <param name="handler">Type of the failed handler</param>
		/// <param name="ex">Exception thrown</param>
		void HandlerFailed(IMessage message, Type handler, Exception ex);
	}
}