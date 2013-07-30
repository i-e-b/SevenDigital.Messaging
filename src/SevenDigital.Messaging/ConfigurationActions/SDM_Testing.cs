using System;
using System.Collections.Generic;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Loopback;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_Testing : IMessagingTestingMethods
	{
		public ITestEvents LoopbackEvents()
		{
			var testHook = ObjectFactory.TryGetInstance<ITestEvents>();
			
			if (testHook == null) throw new InvalidOperationException("Loopback events are not available: Loopback mode has not be set. Try `MessagingSystem.Configure.WithLoopbackMode()` before your service starts.");

			return testHook;
		}

		public ILoopbackBinding LoopbackHandlers()
		{
			var lb = ObjectFactory.TryGetInstance<ILoopbackBinding>();
			if (lb == null) 
				throw new Exception("Loopback lister list is not available: Loopback mode has not be set. Try `MessagingSystem.Configure.WithLoopbackMode()` before your service starts.");

			return lb;
		}

		public void AddTestEventHook()
		{
			ObjectFactory.Configure(map => map.For<ITestEvents>().Singleton().Use<TestEvents>());
			MessagingSystem.Events.AddEventHook<TestEventHook>();
		}

		public int ConcurrencyLimit()
		{
			return MessagingSystem.Concurrency;
		}

		[Obsolete("Use `LoopbackHandlers().ForMessage<T>()` instead")]
		public IEnumerable<Type> LoopbackListenersForMessage<T>()
		{
			var lb = ObjectFactory.TryGetInstance<ILoopbackBinding>();
			if (lb == null) 
				throw new Exception("Loopback lister list is not available: Loopback mode has not be set. Try `MessagingSystem.Configure.WithLoopbackMode()` before your service starts.");

			return lb.ForMessage<T>();
		}
	}
}