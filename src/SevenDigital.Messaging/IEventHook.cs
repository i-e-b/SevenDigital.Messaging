namespace SevenDigital.Messaging
{
	public interface IEventHook
	{
		void MessageSent(IMessage msg);
		void MessageReceived(IMessage msg);
	}
}