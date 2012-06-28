using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class AddressTests
	{
		
		[Test]
		public void Address_contains_rabbitmq_protocol()
		{
			var subject = new Node(new Host(""), new Endpoint(""), null);
			const string rabbitMqProtocol = "rabbitmq://";
			Assert.That(subject.Address.ToString(), Is.StringStarting(rabbitMqProtocol));
		}

		[Test]
		public void Address_contains_host_name()
		{
			const string hostname = "hostyhost";
			var subject = new Node(new Host(hostname), new Endpoint(""), null);
			Assert.That(subject.Address.ToString(), Is.StringContaining(hostname));
		}

		[Test]
		public void Address_contains_endpoint_name()
		{
			const string endpointName = "endypointy";
			var subject = new Node(new Host(""), new Endpoint(endpointName), null);
			Assert.That(subject.Address.ToString(), Is.StringEnding(endpointName));
		}

		[Test]
		public void Address_is_as_expected()
		{
			const string endpointName = "endypointy";
			const string hostName = "hostyhost";
			var subject = new Node(new Host(hostName), new Endpoint(endpointName), null);
			Assert.That(subject.Address.ToString(), Is.EqualTo("rabbitmq://hostyhost/endypointy"));
		}
	}
}