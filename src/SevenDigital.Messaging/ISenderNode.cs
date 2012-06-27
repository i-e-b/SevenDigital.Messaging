using System;
using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Core
{
	public interface ISenderNode : IDisposable
	{
		void SendMessage<T>(T message) where T : class, IMessage;
	}
}