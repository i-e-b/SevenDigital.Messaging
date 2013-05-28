namespace SevenDigital.Messaging.MessageSending.Loopback
{
	/// <summary>
	/// Loopback sender node
	/// </summary>
	public class LoopbackSender : ISenderNode
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;

		/// <summary>
		/// Create a loopback node.
		/// You shouldn't create this yourself.
		/// Use `Messaging.Sender()` in loopback mode
		/// </summary>
		public LoopbackSender(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		public void SendMessage<T>(T message) where T : class, IMessage
		{
			loopbackNodeFactory.Send(message);
		}
	}

	/// <summary>
	/// Loopback receiver node
	/// </summary>
	public class LoopbackReceiver : IReceiverNode
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;
		
		/// <summary>
		/// Create a loopback node.
		/// You shouldn't create this yourself.
		/// Use `Messaging.Receiver()` in loopback mode
		/// </summary>
		public LoopbackReceiver(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		/// <summary>
		/// Stop this node. Takes no action in loopback mode
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="T">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			return new LoopbackBinder<T>(loopbackNodeFactory);
		}

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return ""; } }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="T">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		public void Unregister<T>()
		{
			loopbackNodeFactory.Unregister<T>();
		}
	}

	/// <summary>
	/// Loopback message binder
	/// </summary>
	public class LoopbackBinder<T> : IMessageBinding<T> where T : class, IMessage
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;
		
		/// <summary>
		/// Create a loopback binder.
		/// You shouldn't create this yourself.
		/// Use `Messaging.Receiver().Handle&lt;TMessage&gt;().With&lt;THandler&gt;()` in loopback mode
		/// </summary>
		public LoopbackBinder(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<T>
		{
			loopbackNodeFactory.Bind<T, THandler>();
		}
	}
}
