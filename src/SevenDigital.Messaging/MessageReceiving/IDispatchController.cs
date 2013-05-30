namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Dispatch controller is a simple list
	/// of Destination Pollers with a shut-down switch.
	/// </summary>
	public interface IDispatchController
	{
		/*
		/// <summary>
		/// Create a poller for a given destination name
		/// </summary>
		IDestinationPoller CreatePoller(string destinationName);
		*/
		
		/// <summary>
		/// Add a message/handler binding
		/// </summary>
		void AddHandler<TMessage, THandler>(string destinationName)
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		/// <summary>
		/// Remove a handler from all message bindings
		/// </summary>
		void RemoveHandler<T>(string destinationName);

		/// <summary>
		/// Stop polling
		/// </summary>
		void Shutdown();
	}
}