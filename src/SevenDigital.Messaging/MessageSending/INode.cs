using System;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public interface INode:IDisposable
	{
		void SetEndpoint(IRoutingEndpoint endpoint);
		void SubscribeHandler<T>(Action<T> action) where T : class;
	}
}