using System;
using SevenDigital.Messaging.MessageReceiving;

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
		/// Bind messages to handler types.
		/// </summary>
		public void Register(IBinding bindings)
		{
			foreach (var binding in bindings.AllBindings())
			{
				Type messageType = binding.Item1, handlerType = binding.Item2;
				_loopbackReceiver.Bind(messageType, handlerType);
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