using System;

namespace SevenDigital.Messaging.EventHooks
{
	public class ConsoleEventHook : IEventHook
	{
		public void MessageSent(IMessage msg){
			Console.WriteLine("Sent: "+msg);
		}
		public void MessageReceived(IMessage msg){
			Console.WriteLine("Got: "+msg);
		}
	}
}