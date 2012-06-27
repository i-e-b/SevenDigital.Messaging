using SevenDigital.Messaging.Core.Domain;

namespace SevenDigital.Messaging.Core
{
	public interface INodeFactory
	{
		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		IReceiverNode ListenOn(Endpoint endpoint);

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		IReceiverNode Listener();

		/// <summary>
		/// Get a sender node to push messages to handlers.
		/// </summary>
		ISenderNode Sender();
	}
}