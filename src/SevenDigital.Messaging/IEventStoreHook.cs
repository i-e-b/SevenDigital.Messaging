namespace SevenDigital.Messaging
{
	public interface IEventStoreHook
	{
		void MessageSent(IMessage msg);
		void MessageReceived(IMessage msg);
	}
}