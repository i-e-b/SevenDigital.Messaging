namespace SevenDigital.Messaging.MessageReceiving
{

	/// <summary>
	/// A Destination poller is bound to a single destination queue
	/// and keeps a list of message types that have been registered.
	/// 
	/// When a message is available on the queue, the Message Dispatcher
	/// is called to run all the handlers for the received message type.
	/// </summary>
	public interface IDestinationPoller
	{
		/// <summary>
		/// Set target destinaton
		/// </summary>
		void SetDestinationToWatch(string targetDestination);
		/// <summary>
		/// Start polling
		/// </summary>
		void Start();
		/// <summary>
		/// Stop polling
		/// </summary>
		void Stop();
		/// <summary>
		/// Add a message/handler binding
		/// </summary>
		void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		/// <summary>
		/// Remove a handler from all message bindings
		/// </summary>
		void RemoveHandler<T>();

		/// <summary>
		/// Count of handlers bound
		/// </summary>
		int HandlerCount { get; }
	}
}