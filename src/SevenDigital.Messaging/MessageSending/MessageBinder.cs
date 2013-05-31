namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard handler binding for messaging
	/// </summary>
	/// <typeparam name="TMessage">Message type to bind</typeparam>
	public class MessageBinder<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly IBindingHost _host;

		/// <summary>
		/// create a handler binding api point
		/// </summary>
		public MessageBinder(IBindingHost host)
		{
			_host = host;
		}

		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			_host.BindHandler(typeof(TMessage), typeof(THandler));
		}
	}
}
