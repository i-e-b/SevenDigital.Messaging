using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Generates polling, self filling worker queues
	/// </summary>
	public interface IPollingNodeFactory
	{
		/// <summary>
		/// Create a worker queue for the named endpoint
		/// </summary>
		ITypedPollingNode Create(IRoutingEndpoint endpoint);
	}
}