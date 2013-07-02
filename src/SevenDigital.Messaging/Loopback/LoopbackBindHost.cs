namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// Loopback mode binding host
	/// </summary>
	public class LoopbackBindHost : IMessageBinding
	{
		readonly LoopbackReceiver _loopbackReceiver;

		/// <summary>
		/// Loopback mode binding host
		/// </summary>
		/// <param name="loopbackReceiver"></param>
		public LoopbackBindHost(LoopbackReceiver loopbackReceiver)
		{
			_loopbackReceiver = loopbackReceiver;
		}

		/// <summary>
		/// Bind a message type to a handler type
		/// </summary>
		/// <typeparam name="TMessage">Type of message to handle. This should be an interface that implements IMessage.</typeparam>
		/// <returns>A message binding, use this to specify the handler type</returns>
		public IHandlerBinding<TMessage> Handle<TMessage>() where TMessage : class, IMessage
		{
			return new LoopbackBinder<TMessage>(_loopbackReceiver);
		}
	}
}