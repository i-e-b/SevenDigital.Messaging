using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class NodeFactory : INodeFactory
	{
		private readonly IEndpointGenerator _uniqueEndPointGenerator;

	    public NodeFactory(IUniqueEndpointGenerator uniqueEndPointGenerator)
		{
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
		}

		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			return new ReceiverNode(endpoint);
		}

		public IReceiverNode Listen()
		{
			return new ReceiverNode(_uniqueEndPointGenerator.Generate());
		}
	}
}