using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class SenderTests
	{
		ISenderNode _subject;
		Host _host;
		Mock<ISenderEndpointGenerator> _senderEndPointGenerator;
		Endpoint _senderEndpoint;

		[SetUp]
		public void SetUp()
		{
			_senderEndPointGenerator = new Mock<ISenderEndpointGenerator>();
			_host = new Host("myMachine");
			_senderEndpoint = new Endpoint("a.sender.com");
			_senderEndPointGenerator.Setup(x => x.Generate()).Returns(_senderEndpoint);
            
			_subject = new SenderNode(_host, _senderEndPointGenerator.Object, new DummyStub__ServiceBusFactory());
		}

		[Test]
		public void Sender_should_get_an_endpoint_from_the_provided_generator()
		{
            _senderEndPointGenerator.Verify(x => x.Generate());
		}

		[Test]
		public void Sender_should_create_sender_node_with_unique_endpoint()
		{
            Assert.That(_subject, Is.EqualTo(new SenderNode(_host, _senderEndPointGenerator.Object, null)));
		}
	}
}