using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging
{
	public interface INodeFactory
	{
		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		IReceiverNode TakeFrom(Endpoint endpoint);

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		IReceiverNode Listen();

		/// <summary>
		/// Get a sender node to push messages to handlers.
		/// </summary>
		ISenderNode Sender();
	}
}