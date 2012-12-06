namespace SevenDigital.Messaging.MessageSending.Loopback
{
	public class LoopbackSender : ISenderNode
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;
        
		public LoopbackSender(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		public void SendMessage<T>(T message) where T : class, IMessage
		{
			loopbackNodeFactory.Send(message);
		}
	}

	public class LoopbackReceiver : IReceiverNode
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;

		public LoopbackReceiver(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		public void Dispose(){}

		public IMessageBinding<T> Handle<T>() where T : class, IMessage
		{
			return new LoopbackBinder<T>(loopbackNodeFactory);
		}

		public string DestinationName { get { return ""; }}
	}

	public class LoopbackBinder<T>:IMessageBinding<T> where T : class, IMessage
	{
		readonly LoopbackNodeFactory loopbackNodeFactory;

		public LoopbackBinder(LoopbackNodeFactory loopbackNodeFactory)
		{
			this.loopbackNodeFactory = loopbackNodeFactory;
		}

		public void With<THandler>() where THandler : IHandle<T>
		{
			loopbackNodeFactory.Bind<T, THandler>();
		}
	}
}
