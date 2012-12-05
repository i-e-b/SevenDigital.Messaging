using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class NodeFactory : INodeFactory
	{
		private readonly IMessagingHost _host;
		private readonly IEndpointGenerator _uniqueEndPointGenerator;
	    private readonly IServiceBusFactory _serviceBusFactory;

	    public NodeFactory(IMessagingHost host, IUniqueEndpointGenerator uniqueEndPointGenerator, ISenderEndpointGenerator senderEndpoint, IServiceBusFactory serviceBusFactory)
		{
			_host = host;
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
	        _serviceBusFactory = serviceBusFactory;
		}

		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			return new ReceiverNode(_host, endpoint, _serviceBusFactory);
		}

		public IReceiverNode Listen()
		{
			return new ReceiverNode(_host, _uniqueEndPointGenerator.Generate(), _serviceBusFactory);
		}
	}
}