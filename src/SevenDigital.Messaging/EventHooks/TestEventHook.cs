using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging.EventHooks
{
	public class TestEventHook : ITestEventHook
	{
		readonly List<IMessage> sentMessages = new List<IMessage>();
		readonly List<IMessage> receivedMessages = new List<IMessage>();
		readonly List<Exception> handlerExceptions = new List<Exception>();

		public void MessageSent(IMessage message, string serialisedMessage, string contractTypeName)
		{
			sentMessages.Add(message);
		}

		public void MessageReceived(IMessage message, string contractTypeName)
		{
			receivedMessages.Add(message);
		}

		public void HandlerFailed(IMessage message, Type handler, Exception ex)
		{
			handlerExceptions.Add(ex);
		}

		public IEnumerable<IMessage> SentMessages { get { return sentMessages; } }
		public IEnumerable<IMessage> ReceivedMessages { get { return receivedMessages; } }
		public IEnumerable<Exception> HandlerExceptions { get { return handlerExceptions; } }
		
		public void Reset()
		{
			sentMessages.Clear();
			receivedMessages.Clear();
			handlerExceptions.Clear();
		}
	}
}