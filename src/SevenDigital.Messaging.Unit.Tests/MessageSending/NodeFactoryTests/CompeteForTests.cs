using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class CompeteForTests
	{
		Endpoint _endpoint;
		IReceiverNode _result;
		Mock<INode> mockNode;

		[SetUp]
		public void SetUp()
		{
			var uniqueEndPointGenerator = new Mock<IUniqueEndpointGenerator>();
			mockNode = new Mock<INode>();
			ObjectFactory.Configure(map=>map.For<INode>().Use(mockNode.Object));
			
			_endpoint = new Endpoint("doStuff");
			
			var subject = new NodeFactory(uniqueEndPointGenerator.Object);

			_result = subject.TakeFrom(_endpoint);
		}

		[Test]
		public void Compete_for_should_create_new_reciever_node_for_node_name()
		{
			Assert.That(_result, Is.EqualTo(new ReceiverNode(_endpoint)));
		}
	}
}