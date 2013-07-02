using System;

namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// Loopback receiver node
	/// </summary>
	public class LoopbackReceiverNode : IReceiverNode
	{
		readonly LoopbackReceiver _loopbackReceiver;
		
		/// <summary>
		/// Create a loopback node.
		/// You shouldn't create this yourself.
		/// Use `Messaging.Receiver()` in loopback mode
		/// </summary>
		public LoopbackReceiverNode(LoopbackReceiver _loopbackReceiver)
		{
			this._loopbackReceiver = _loopbackReceiver;
		}

		/// <summary>
		/// Stop this node. Takes no action in loopback mode
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="T">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		[Obsolete("This configuration method has a race condition. Please use `Register(b=>b.Handle<message>().With<>())` instead")]
		public IHandlerBinding<T> Handle<T>() where T : class, IMessage
		{
			return new LoopbackBinder<T>(_loopbackReceiver);
		}

		/// <summary>
		/// Bind messages to handler types.
		/// </summary>
		public void Register(params Action<IMessageBinding>[] bindings)
		{
			foreach (var binding in bindings)
			{
				binding(new LoopbackBindHost(_loopbackReceiver));
			}
		}

		/// <summary>
		/// Gets the name of the destination queue used by messaging
		/// </summary>
		public string DestinationName { get { return ""; } }

		/// <summary>
		/// Remove a handler from all message bindings. The handler will no longer be called.
		/// </summary>
		/// <typeparam name="T">Type of hander previously bound with `Handle&lt;T&gt;`</typeparam>
		public void Unregister<T>()
		{
			_loopbackReceiver.Unregister<T>();
		}

		/// <summary>
		/// Set maximum number of concurrent handlers on this node
		/// </summary>
		public void SetConcurrentHandlers(int max)
		{
		}
	}
}