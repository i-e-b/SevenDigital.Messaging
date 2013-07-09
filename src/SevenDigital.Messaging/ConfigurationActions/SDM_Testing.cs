using System;
using System.Collections.Generic;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Loopback;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_Testing : IMessagingLoopbackInformation
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