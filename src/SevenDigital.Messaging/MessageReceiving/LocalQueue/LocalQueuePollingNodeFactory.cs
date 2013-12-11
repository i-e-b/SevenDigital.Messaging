using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.ConfigurationActions;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageReceiving.LocalQueue
{
	public class LocalQueuePollingNodeFactory:IPollingNodeFactory
	{
		readonly string _dispatchPath;
		readonly IMessageSerialiser _serialiser;
		readonly ISleepWrapper _sleeper;
		readonly string _incomingPath;

		public LocalQueuePollingNodeFactory(LocalQueueConfig config,
		                                    IMessageSerialiser serialiser, ISleepWrapper sleeper)
		{
			_serialiser = serialiser;
			_sleeper = sleeper;
			_dispatchPath = config.DispatchPath;
			_incomingPath = config.IncomingPath;
		}

		public ITypedPollingNode Create(IRoutingEndpoint endpoint)
		{
			return new LocalQueuePollingNode(_dispatchPath, _incomingPath, _serialiser, _sleeper);
		}
	}
}