using NUnit.Framework;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class ConfigureWithMessageQueryTests
	{
		IRabbitMqQuery subject;

		[TestFixtureSetUp]
		public void Configured_with_message_query ()
		{
			Messaging.Configure.WithDefaults().SetManagementServer("host", "user", "pass", "vhost");
			subject = ObjectFactory.GetInstance<IRabbitMqQuery>();
		}

		[Test]
		public void Should_get_query_with_configured_host ()
		{
			Assert.That(subject.HostUri.ToString(), Is.EqualTo("http://host:55672/"));
		}

		[Test]
		public void Should_get_host_with_configured_vhost ()
		{
			Assert.That(subject.VirtualHost, Is.EqualTo("/vhost"));
		}

		[Test]
		public void Should_get_host_with_configured_credentials ()
		{
			Assert.That(subject.Credentials.Password, Is.EqualTo("pass"));
			Assert.That(subject.Credentials.UserName, Is.EqualTo("user"));
		}
	}
}
