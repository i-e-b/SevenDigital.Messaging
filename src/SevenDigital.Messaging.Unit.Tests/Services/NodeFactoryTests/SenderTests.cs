using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Domain;
using SevenDigital.Messaging.Services;

namespace SevenDigital.Messaging.Unit.Tests.Services.NodeFactoryTests
{
	[TestFixture]
	public class SenderTests
	{
		INodeFactory _subject;
		Host _host;
		Mock<IEndpointGenerator> _uniqueEndPointGenerator;
		ISenderNode _result;
		Endpoint _uniqueEndpoint;

		[SetUp]
		public void SetUp()
		{
			_uniqueEndPointGenerator = new Mock<IEndpointGenerator>();
			_host = new Host("myMachine");
			_subject = new NodeFactory(_host, _uniqueEndPointGenerator.Object, new ServiceBusFactory());
			_uniqueEndpoint = new Endpoint("some wordz");
			_uniqueEndPointGenerator.Setup(x => x.Generate()).Returns(_uniqueEndpoint);

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
			_uniqueEndPointGenerator.Verify(x => x.Generate());
		}

		[Test]
		public void Sender_should_create_sender_node_with_unique_endpoint()
		{
			Assert.That(_result, Is.EqualTo(new SenderNode(_host, _uniqueEndpoint, null)));
		}
	}
}