using System;

namespace SevenDigital.Messaging.EventHooks
{
	public class ConsoleEventHook : IEventHook
	{
		public void MessageSent(IMessage message)
		{
			Console.WriteLine("Sent: " + message);
		}
		public void MessageReceived(IMessage message)
		{
			Console.WriteLine("Got: " + message);
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			Console.WriteLine("Handler '" + handler.Name + "' failed on message '" + message + "' throwing: " + ex.Message);
		}
	}
}