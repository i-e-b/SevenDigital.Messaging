using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// A messaging node that can receive messages and pass them to handlers
	/// </summary>
	public interface IReceiverNode : IDisposable
	{
		/// <summary>
		/// Bind multiple messages to handler types.
		/// </summary>
		void Register(IEnumerable<Tuple<Type,Type>> bindings);

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		string DestinationName { get; }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="THandler">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		void Unregister<THandler>();

		/// <summary>
		/// Set maximum number of concurrent handlers on this node
		/// </summary>
		void SetConcurrentHandlers(int max);
	}
}