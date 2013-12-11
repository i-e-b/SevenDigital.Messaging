using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_LocalQueueOptions : ILocalQueueOptions
	{
		public ILocalQueueOptions SendHandlerErrorsToQueue(string errorQueueStorage)
		{
			ObjectFactory.Configure(map => map
				.For<IEventHook>()
				.Add<LocalQueueExceptionHook>()
				.Ctor<string>().Is(errorQueueStorage));

			return this;
		}
	}
}