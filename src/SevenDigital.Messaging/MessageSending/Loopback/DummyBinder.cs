namespace SevenDigital.Messaging.MessageSending.Loopback
{
	public class DummyReceiver : IReceiverNode
	{
		public void Dispose(){}

		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			return new DummyBinding<T>();
		}

		public string DestinationName { get; private set; }
	}
	
	public class DummyBinding<T> : IMessageBinding<T> where T : class, IMessage
	{
		public void With<THandler>() where THandler : IHandle<T>{}
	}
}