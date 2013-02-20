using System;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging
{
	public interface IReceiverNode :IDisposable
	{
		IMessageBinding<T> Handle<T>() where T : class, IMessage;
		string DestinationName { get; }
		void Unregister<T>();
	}
}