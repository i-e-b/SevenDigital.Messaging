using System;
using SevenDigital.Messaging.Services;
using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging
{
	public interface IReceiverNode : IDisposable
	{
		MessageBinding<T> Handle<T>() where T : class, IMessage;
	}
}