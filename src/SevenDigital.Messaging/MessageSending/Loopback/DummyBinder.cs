namespace SevenDigital.Messaging.MessageSending.Loopback
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
		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			return new DummyBinding<T>();
		}

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return "DummyDestination"; } }
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Unregister<T>() { }
	}
	
	/// <summary>
	/// Binder for dummy node that does nothing.
	/// </summary>
	public class DummyBinding<T> : IMessageBinding<T> where T : class, IMessage
	{
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<T>{}
	}
}