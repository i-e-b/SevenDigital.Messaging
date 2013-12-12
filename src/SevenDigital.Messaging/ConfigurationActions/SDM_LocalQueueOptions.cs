using System.IO;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_LocalQueueOptions : ILocalQueueOptions
	{
		public ILocalQueueOptions SendHandlerErrorsToQueue(string errorQueueStorage)
		{
			LocalQueueExceptionHook.Inject(errorQueueStorage);
			return this;
		}

		public ILocalQueueOptions SendTo(string writeQueuePath)
		{
			
			lock (MessagingSystem.ConfigurationLock)
			{
				var oldConfig = ObjectFactory.GetInstance<LocalQueueConfig>();
				ObjectFactory.EjectAllInstancesOf<LocalQueueConfig>();

				ObjectFactory.Configure(map =>
					map
						.For<LocalQueueConfig>()
						.Use(new LocalQueueConfig
						{
							DispatchPath = oldConfig.DispatchPath,
							IncomingPath = oldConfig.IncomingPath,
							WritePath = Path.Combine(writeQueuePath, LocalQueueConfig.IncomingQueueSubpath),
						}
						)
					);
			}
			return this;
		}
	}
}