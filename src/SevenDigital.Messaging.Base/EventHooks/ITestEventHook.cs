using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging.EventHooks
{
	public interface ITestEventHook : IEventHook
	{
		IEnumerable<IMessage> SentMessages { get; }
		IEnumerable<IMessage> ReceivedMessages { get; }
		IEnumerable<Exception> HandlerExceptions { get; }

		void Reset();
	}
}