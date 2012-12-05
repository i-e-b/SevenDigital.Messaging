namespace SevenDigital.Messaging.MessageSending
{
	public interface IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		void With<THandler>() where THandler : IHandle<TMessage>;
	}
}