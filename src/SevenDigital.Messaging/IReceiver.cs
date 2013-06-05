using System;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// A receiver node source that can produce competitive or co-operative consumers
	/// </summary>
	public interface IReceiver
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

	}

	/// <summary>
	/// Control methods for IReceiver.
	/// Seperated to simplify receiver interface.
	/// </summary>
	public interface IReceiverControl: IDisposable
	{
		/// <summary>
		/// Close all receiver nodes that have been created
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Unregister a node from the shutdown list
		/// </summary>
		void Remove(IReceiverNode node);

		/// <summary>
		/// Set maximum concurrent handlers per receiver node
		/// </summary>
		void SetConcurrentHandlers(int max);

		/// <summary>
		/// Set purging policy. If true, all waiting messages are DELETED when a handler is registered.
		/// This setting is meant for integration tests.
		/// Default is false.
		/// </summary>
		bool PurgeOnConnect { get; set; }

		/// <summary>
		/// Set cleanup policy. If true, all endpoints generated in integration mode
		/// are deleted when the receiver is disposed.
		/// Default is false.
		/// </summary>
		bool DeleteIntegrationEndpointsOnShutdown { get; set; }
	}
}