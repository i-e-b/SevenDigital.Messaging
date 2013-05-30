using System;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard handler binding for messaging
	/// </summary>
	/// <typeparam name="TMessage">Message type to bind</typeparam>
	public class HandlerTriggering<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		/// <summary>
		/// Create a new binding. You should no call this yourself.
		/// Use `Messaging.Receiver().Handle&lt;TMessage&gt;().With&lt;THandler&gt;()`
		/// </summary>
		public HandlerTriggering()
		{
		}

		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			//listenerNode.SubscribeHandler<TMessage, THandler>();
			throw new NotImplementedException("Needs to be connected to incoming dispatcher");
		}
	}
}
