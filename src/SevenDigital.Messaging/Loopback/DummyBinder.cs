using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// A receiver node that does nothing.
	/// </summary>
	public class DummyReceiver : IReceiverNode
	{
		/// <summary>
		/// No action in dummy
		/// </summary>
		public void Dispose(){}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Register(IBinding bindings) { }

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return "DummyDestination"; } }
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Unregister<T>() { }
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void SetConcurrentHandlers(int max) { }
	}
}