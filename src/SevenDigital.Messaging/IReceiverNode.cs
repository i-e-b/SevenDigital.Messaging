using System;
using SevenDigital.Messaging.Core.Services;
using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Core
{
	public interface IReceiverNode : IDisposable
	{
		MessageBinding<T> Handle<T>() where T : class, IMessage;
	}
}