namespace SevenDigital.Messaging.EventStoreHooks
{
	public class NoEventStoreHook : IEventStoreHook
	{
		public void MessageSent(IMessage msg){}
		public void MessageReceived(IMessage msg){}
	}
}