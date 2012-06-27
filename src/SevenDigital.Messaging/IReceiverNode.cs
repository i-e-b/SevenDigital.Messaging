using System;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging
{
	public interface IReceiverNode : IDisposable
	{
		MessageBinding<T> Handle<T>() where T : class, IMessage;
	}
}