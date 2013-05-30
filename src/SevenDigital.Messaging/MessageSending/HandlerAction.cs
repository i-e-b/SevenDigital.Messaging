using System;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Generic action performed by a handler. This maps to `IHandle&lt;T&gt;`
	/// </summary>
	public delegate Exception HandlerAction<T>(T message) where T : class, IMessage;
}