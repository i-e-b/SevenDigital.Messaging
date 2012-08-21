namespace SevenDigital.Messaging
{
	public interface IHandle<T> where T : IMessage
	{
		void Handle(T message);
	}
}