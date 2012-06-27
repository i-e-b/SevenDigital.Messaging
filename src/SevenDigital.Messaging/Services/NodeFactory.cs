using SevenDigital.Messaging.Domain;

namespace SevenDigital.Messaging.Services
{
	public class NodeFactory : INodeFactory
	{
		private readonly Host _host;
		private readonly IEndpointGenerator _uniqueEndPointGenerator;
		private readonly IServiceBusFactory _serviceBusFactory;

		public NodeFactory(Host host, IEndpointGenerator uniqueEndPointGenerator, IServiceBusFactory serviceBusFactory)
		{
			_host = host;
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
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
			return new SenderNode(_host, _uniqueEndPointGenerator.Generate(), _serviceBusFactory);
		}

		public ISenderNode SendOn(Endpoint endpoint)
		{
			return new SenderNode(_host, endpoint, _serviceBusFactory);
		}
	}
}