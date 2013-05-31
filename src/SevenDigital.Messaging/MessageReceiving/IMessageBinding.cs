namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// A message binder, used to bind handlers to message types in a receiver node
	/// </summary>
	/// <typeparam name="TMessage">Message type to be bound</typeparam>
	public interface IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		void With<THandler>() where THandler : IHandle<TMessage>;
	}
}