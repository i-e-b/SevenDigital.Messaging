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
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
		void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		void RemoveHandler<T>();
		int HandlerCount { get; }
	}
}