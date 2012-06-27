using System;

namespace SevenDigital.Messaging
{
	public interface ISenderNode : IDisposable
	{
		void SendMessage<T>(T message) where T : class, IMessage;
	}
}