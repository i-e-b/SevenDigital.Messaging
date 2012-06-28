using System;

namespace SevenDigital.Messaging
{
	public interface IEventHook
	{
		void MessageSent(IMessage message);
		void MessageReceived(IMessage message);
		void HandlerFailed(IMessage message, Type handler, Exception ex);
	}
}