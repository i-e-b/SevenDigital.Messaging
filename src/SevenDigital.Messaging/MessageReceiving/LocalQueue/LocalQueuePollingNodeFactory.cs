using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.ConfigurationActions;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageReceiving.LocalQueue
{
	/// <summary>
	/// Default LocalQueue polling node factory
	/// </summary>
	public class LocalQueuePollingNodeFactory:IPollingNodeFactory
	{
		readonly string _dispatchPath;
		readonly IMessageSerialiser _serialiser;
		readonly ISleepWrapper _sleeper;
		readonly string _incomingPath;

		/// <summary>
		/// Create a default local queue polling node factory
		/// <para>You should not use this yourself. Use:</para>
		/// <para>MessagingSystem.Configure.WithLocalQueue(...);</para>
		/// </summary>
		public LocalQueuePollingNodeFactory(LocalQueueConfig config,
		                                    IMessageSerialiser serialiser, ISleepWrapper sleeper)
		{
			_serialiser = serialiser;
			_sleeper = sleeper;
			_dispatchPath = config.DispatchPath;
			_incomingPath = config.IncomingPath;
		}

		/// <summary>
		/// Create a worker queue for the named endpoint.
		/// <para>endpoint names are ignored for local queues</para>
		/// </summary>
		public ITypedPollingNode Create(IRoutingEndpoint endpoint)
		{
			return new LocalQueuePollingNode(_dispatchPath, _incomingPath, _serialiser, _sleeper);
		}
	}
}