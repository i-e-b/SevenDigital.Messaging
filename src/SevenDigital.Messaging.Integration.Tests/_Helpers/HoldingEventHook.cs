using System;
using System.Threading;

namespace SevenDigital.Messaging.Integration.Tests._Helpers
{
	class HoldingEventHook : IEventHook
	{
		public IMessage sent, received;
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void MessageSent(IMessage msg)
		{
			sent = msg;
		}

		public void MessageReceived(IMessage msg)
		{
			received = msg;
			AutoResetEvent.Set();
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex){}
	}
}