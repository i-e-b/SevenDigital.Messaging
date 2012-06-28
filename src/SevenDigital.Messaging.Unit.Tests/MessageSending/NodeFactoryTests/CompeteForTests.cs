using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeFactoryTests
{
	[TestFixture]
	public class CompeteForTests
	{
		Host _host;
		Endpoint _endpoint;
		IReceiverNode _result;

		[SetUp]
		public void SetUp()
		{
			var uniqueEndPointGenerator = new Mock<IUniqueEndpointGenerator>();
			var senderEndPointGenerator = new Mock<ISenderEndpointGenerator>();
			
			_host = new Host("myMachine");
			_endpoint = new Endpoint("doStuff");
			
			var subject = new NodeFactory(_host, uniqueEndPointGenerator.Object, senderEndPointGenerator.Object, new ServiceBusFactory());

			_result = subject.ListenOn(_endpoint);
		}

		[Test]
		public void Compete_for_should_create_new_reciever_node_for_node_name()
		{
			Assert.That(_result, Is.EqualTo(new ReceiverNode(_host, _endpoint, null)));
		}
	}
}