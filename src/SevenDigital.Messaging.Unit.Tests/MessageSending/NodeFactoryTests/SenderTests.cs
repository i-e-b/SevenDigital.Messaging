using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class SenderTests
	{
		INodeFactory _subject;
		Host _host;
		Mock<IUniqueEndpointGenerator> _uniqueEndPointGenerator;
		Mock<ISenderEndpointGenerator> _senderEndPointGenerator;
		ISenderNode _result;
		Endpoint _senderEndpoint;

		[SetUp]
		public void SetUp()
		{
			_uniqueEndPointGenerator = new Mock<IUniqueEndpointGenerator>();
			_senderEndPointGenerator = new Mock<ISenderEndpointGenerator>();
			_host = new Host("myMachine");
			_senderEndpoint = new Endpoint("a.sender.com");
			_senderEndPointGenerator.Setup(x => x.Generate()).Returns(_senderEndpoint);


			_subject = new NodeFactory(_host, _uniqueEndPointGenerator.Object, _senderEndPointGenerator.Object, new ServiceBusFactory());
			
			_result = _subject.Sender();
		}

		[Test]
		public void Sender_should_create_sender_node()
		{
			Assert.That(_result, Is.TypeOf<SenderNode>());
		}

		[Test]
		public void Sender_should_get_a_unique_endpoint()
		{
			_senderEndPointGenerator.Verify(x => x.Generate());
		}

		[Test]
		public void Sender_should_create_sender_node_with_unique_endpoint()
		{
			Assert.That(_result, Is.EqualTo(new SenderNode(_host, _senderEndpoint, null)));
		}
	}
}