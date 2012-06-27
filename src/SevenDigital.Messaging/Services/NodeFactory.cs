using SevenDigital.Messaging.Core.Domain;

namespace SevenDigital.Messaging.Core.Services
{
	public class NodeFactory : INodeFactory
	{
		private readonly IMessagingHost _host;
		private readonly IEndpointGenerator _uniqueEndPointGenerator;
		readonly ISenderEndpointGenerator _senderEndpointGenerator;
		private readonly IServiceBusFactory _serviceBusFactory;

		public NodeFactory(IMessagingHost host, IUniqueEndpointGenerator uniqueEndPointGenerator, ISenderEndpointGenerator senderEndpoint, IServiceBusFactory serviceBusFactory)
		{
			_host = host;
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
			_senderEndpointGenerator = senderEndpoint;
			_serviceBusFactory = serviceBusFactory;
		}

		public IReceiverNode ListenOn(Endpoint endpoint)
		{
			return new ReceiverNode(_host, endpoint, _serviceBusFactory);
		}

		public IReceiverNode Listener()
		{
			return new ReceiverNode(_host, _uniqueEndPointGenerator.Generate(), _serviceBusFactory);
		}

		public ISenderNode Sender()
		{
			return new SenderNode(_host, _senderEndpointGenerator.Generate(), _serviceBusFactory);
		}
	}
}