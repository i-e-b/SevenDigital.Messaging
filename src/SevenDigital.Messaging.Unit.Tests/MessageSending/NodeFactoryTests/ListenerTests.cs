using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class ListenerTests
	{
		INodeFactory _subject;
		Host _host;
		Mock<IUniqueEndpointGenerator> _uniqueEndPointGenerator;
		Mock<ISenderEndpointGenerator> _senderEndPointGenerator;
		IReceiverNode _result;
		Endpoint _uniqueEndpoint;

		[SetUp]
		public void SetUp()
		{
			_uniqueEndPointGenerator = new Mock<IUniqueEndpointGenerator>();
			_senderEndPointGenerator = new Mock<ISenderEndpointGenerator>();
			_host = new Host("myMachine");
			_subject = new NodeFactory(_host, _uniqueEndPointGenerator.Object, _senderEndPointGenerator.Object, new ServiceBusFactory());
			_uniqueEndpoint = new Endpoint("some wordz");
			_uniqueEndPointGenerator.Setup(x => x.Generate()).Returns(_uniqueEndpoint);

			_result = _subject.Listener();
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
			Assert.That(_result, Is.EqualTo(new ReceiverNode(_host, _uniqueEndpoint, null)));
		}
	}
}