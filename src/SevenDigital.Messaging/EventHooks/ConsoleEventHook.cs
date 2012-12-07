using System;

namespace SevenDigital.Messaging.EventHooks
{
	public class ConsoleEventHook : IEventHook
	{
		public void MessageSent(IMessage message, string serialisedMessage, string contractTypeName)
		{
			Console.WriteLine("Sent: " + message + " contract: " + contractTypeName);
		}
		public void MessageReceived(IMessage message, string contractTypeName)
		{
			Console.WriteLine("Got: " + message + " contract: " + contractTypeName);
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			Console.WriteLine("Handler '" + handler.Name + "' failed on message '" + message + "' throwing: " + ex.Message);
		}
	}
}