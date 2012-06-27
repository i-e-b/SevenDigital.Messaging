using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Domain;
using SevenDigital.Messaging.Services;

namespace SevenDigital.Messaging.Unit.Tests.Services.NodeFactoryTests
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
			var uniqueEndPointGenerator = new Mock<IEndpointGenerator>();
			
			_host = new Host("myMachine");
			_endpoint = new Endpoint("doStuff");
			
			var subject = new NodeFactory(_host, uniqueEndPointGenerator.Object, new ServiceBusFactory());

			_result = subject.ListenOn(_endpoint);
		}

		[Test]
		public void Compete_for_should_create_new_reciever_node_for_node_name()
		{
			Assert.That(_result, Is.EqualTo(new ReceiverNode(_host, _endpoint, null)));
		}
	}
}