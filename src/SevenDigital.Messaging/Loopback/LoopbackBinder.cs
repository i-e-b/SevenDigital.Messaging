namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// Loopback message binder
	/// </summary>
	public class LoopbackBinder<T> : IMessageBinding<T> where T : class, IMessage
	{
		readonly LoopbackReceiver _loopbackReceiver;
		
		/// <summary>
		/// Create a loopback binder.
		/// You shouldn't create this yourself.
		/// Use `Messaging.Receiver().Handle&lt;TMessage&gt;().With&lt;THandler&gt;()` in loopback mode
		/// </summary>
		public LoopbackBinder(LoopbackReceiver _loopbackReceiver)
		{
			this._loopbackReceiver = _loopbackReceiver;
		}

		/// <summary>
		/// Bind this handler to receive the selected message type.
		/// The handler may receive any number of messages immediately after calling this method
		/// until unbound or messaging is paused or shutdown.
		/// </summary>
		public void With<THandler>() where THandler : IHandle<T>
		{
			_loopbackReceiver.Bind<T, THandler>();
		}
	}
}