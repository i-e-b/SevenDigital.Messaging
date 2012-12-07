using System;

namespace SevenDigital.Messaging
{
	public interface IEventHook
	{
		void MessageSent(IMessage message, string serialisedMessage, string contractTypeName);
		void MessageReceived(IMessage message, string contractTypeName);
		void HandlerFailed(IMessage message, Type handler, Exception ex);
	}
}