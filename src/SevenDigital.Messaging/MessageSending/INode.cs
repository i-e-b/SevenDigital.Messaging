using System;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
    public delegate Exception HandlerAction<T>(T message) where T : class, IMessage;

	public interface INode : IDisposable
	{
		void SetEndpoint(IRoutingEndpoint endpoint);
		void SubscribeHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>;

		void RemoveHandler<T>();
	}
}