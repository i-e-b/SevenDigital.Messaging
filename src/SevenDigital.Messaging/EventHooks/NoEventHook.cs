namespace SevenDigital.Messaging.EventStoreHooks
{
	public class NoEventHook : IEventHook
	{
		public void MessageSent(IMessage msg){}
		public void MessageReceived(IMessage msg){}
	}
}