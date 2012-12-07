using System;
using System.Threading;

namespace SevenDigital.Messaging.Integration.Tests
{
	class HoldingEventHook : IEventHook
	{
		public IMessage sent, received;
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void MessageSent(IMessage msg, string x, string y)
		{
			sent = msg;
		}

		public void MessageReceived(IMessage msg, string x)
		{
			received = msg;
			AutoResetEvent.Set();
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex){}
	}
}