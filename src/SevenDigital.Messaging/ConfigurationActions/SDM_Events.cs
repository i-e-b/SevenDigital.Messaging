using SevenDigital.Messaging.EventHooks;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_Events : IMessagingEventOptions
	{
		public IMessagingEventOptions ClearEventHooks()
		{
			var loopback = MessagingSystem.UsingLoopbackMode();
			ObjectFactory.EjectAllInstancesOf<IEventHook>();

			if (loopback)
			{
				MessagingSystem.Events.AddEventHook<TestEventHook>();
			}
			return this;
		}

		public IMessagingEventOptions AddEventHook<T>() where T : IEventHook
		{
			ObjectFactory.Configure(map => map.For<IEventHook>().Add<T>());
			return this;
		}
	}
}