using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class ListenerTests
	{
		IReceiver _subject;
		Mock<IUniqueEndpointGenerator> _uniqueEndPointGenerator;
		IReceiverNode _result;
		Endpoint _uniqueEndpoint;
		Mock<INode> mockNode;

		[SetUp]
		public void SetUp()
		{
			mockNode = new Mock<INode>();
			ObjectFactory.Configure(map=>map.For<INode>().Use(mockNode.Object));
			_uniqueEndPointGenerator = new Mock<IUniqueEndpointGenerator>();
			_subject = new Receiver(_uniqueEndPointGenerator.Object);
			_uniqueEndpoint = new Endpoint("some wordz");
			_uniqueEndPointGenerator.Setup(x => x.Generate()).Returns(_uniqueEndpoint);

			_result = _subject.Listen();
		}

		[Test]
		public void Listener_should_create_receiver_node()
		{
			Assert.That(_result, Is.TypeOf<ReceiverNode>());
		}

		[Test]
		public void Listener_should_get_a_unique_endpoint()
		{
			_uniqueEndPointGenerator.Verify(x => x.Generate());
		}

		[Test]
		public void Listener_should_create_receiver_node_with_unique_endpoint()
		{
			Assert.That(_result, Is.EqualTo(new ReceiverNode(_uniqueEndpoint)));
		}
	}
}