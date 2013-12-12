namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_LocalQueueOptions : ILocalQueueOptions
	{
		public ILocalQueueOptions SendHandlerErrorsToQueue(string errorQueueStorage)
		{
			LocalQueueExceptionHook.Inject(errorQueueStorage);
			return this;
		}
	}
}