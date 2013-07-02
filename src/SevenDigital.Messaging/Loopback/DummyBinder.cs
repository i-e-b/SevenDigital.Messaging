using System;

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
		public IHandlerBinding<T> Handle<T>() where T : class, IMessage
		{
			return new DummyBinding<T>();
		}
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void Register(params Action<IMessageBinding>[] bindings)
		{
		}

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
	
	/// <summary>
	/// Binder for dummy node that does nothing.
	/// </summary>
	public class DummyBinding<T> : IHandlerBinding<T> where T : class, IMessage
	{
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<T>{}
	}
}