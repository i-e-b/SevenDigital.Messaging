using System;
using SevenDigital.Messaging.Base.Routing;

namespace SevenDigital.Messaging.MessageReceiving.LocalQueue
{
	public class DummyMessageRouter:IMessageRouter
	{
		public void AddSource(string name) { }
		public void AddBroadcastSource(string className) { }
		public void AddDestination(string name) { }
		public void Link(string sourceName, string destinationName) { } 
		public void RouteSources(string child, string parent) { } 
		public void Send(string sourceName, string data) { } 
		public string Get(string destinationName, out ulong deliveryTag) { deliveryTag = 0; return null; } 
		public void Finish(ulong deliveryTag) { } 
		public string GetAndFinish(string destinationName) { return null; }
		public void Purge(string destinationName) { } 
		public void Cancel(ulong deliveryTag) { } 
		public void RemoveRouting(Func<string, bool> filter) { }
	}
}