using System;
using SevenDigital.Messaging.Base.Routing;

namespace SevenDigital.Messaging.MessageReceiving.LocalQueue
{
	/// <summary>
	/// IMessageRouter that does nothing
	/// </summary>
	public class DummyRouter : IMessageRouter
	{
		/** No op */public void AddSource(string name) { } 
		/** No op */public void AddBroadcastSource(string className) { } 
		/** No op */public void AddDestination(string name) { } 
		/** No op */public void Link(string sourceName, string destinationName) { } 
		/** No op */public void RouteSources(string child, string parent) { } 
		/** No op */public void Send(string sourceName, string data) { } 
		/** No op */public string Get(string destinationName, out ulong deliveryTag) { deliveryTag = 0; return null; } 
		/** No op */public void Finish(ulong deliveryTag) { } 
		/** No op */public string GetAndFinish(string destinationName) { return null; } 
		/** No op */public void Purge(string destinationName) { } 
		/** No op */public void Cancel(ulong deliveryTag) { } 
		/** No op */public void RemoveRouting(Func<string, bool> filter) { }
	}
}