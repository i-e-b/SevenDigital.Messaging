using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class NodeFactory : INodeFactory
	{
		private readonly IMessagingHost _host;
		private readonly IEndpointGenerator _uniqueEndPointGenerator;
	    private readonly IDispatchInterface dispatchInterface;

	    public NodeFactory(IMessagingHost host, IUniqueEndpointGenerator uniqueEndPointGenerator, ISenderEndpointGenerator senderEndpoint, IDispatchInterface dispatchInterface)
		{
			_host = host;
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
	        this.dispatchInterface = dispatchInterface;
		}

		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			return new ReceiverNode(_host, endpoint, dispatchInterface);
		}

		public IReceiverNode Listen()
		{
			return new ReceiverNode(_host, _uniqueEndPointGenerator.Generate(), dispatchInterface);
		}
	}
}