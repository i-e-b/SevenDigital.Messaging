using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard node factory for messaging.
	/// You don't need to create this yourself, use `Messaging.Receiver()`
	/// </summary>
	public class Receiver : IReceiver
	{
		private readonly IEndpointGenerator _uniqueEndPointGenerator;

		/// <summary>
		/// Create a new node factory.
		/// You don't need to create this yourself, use `Messaging.Receiver()`
		/// </summary>
		public Receiver(IUniqueEndpointGenerator uniqueEndPointGenerator)
		{
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
		}

		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			return new ReceiverNode(endpoint);
		}

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		public IReceiverNode Listen()
		{
			return new ReceiverNode(_uniqueEndPointGenerator.Generate());
		}
	}
}