using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageReceiving.RabbitPolling
{
	/// <summary>
	/// Polling node that reads from MessagingBase
	/// </summary>
	public class RabbitMqPollingNodeFactory: IPollingNodeFactory
	{
		readonly IMessagingBase _messagingBase;
		readonly ISleepWrapper _sleeper;

		/// <summary>
		/// Generate a polling node factory for RabbitMq
		/// </summary>
		public RabbitMqPollingNodeFactory(IMessagingBase messagingBase, ISleepWrapper sleeper)
		{
			_messagingBase = messagingBase;
			_sleeper = sleeper;
		}

		/// <summary>
		/// Create a Messaging Base driven polling work queue
		/// </summary>
		public ITypedPollingNode Create(IRoutingEndpoint endpoint)
		{
			return new RabbitMqPollingNode(endpoint, _messagingBase, _sleeper);
		}
	}
}