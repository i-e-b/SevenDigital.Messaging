using NUnit.Framework;
using SevenDigital.Messaging.MessageSending.Loopback;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class StructureMapWithLoopbackTests
	{
		const string HostName = "my.unique.host";

		[SetUp]
		public void Setup()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(HostName);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			ObjectFactory.Container.Dispose();
		}

		[Test]
		public void Should_get_node_factory_implementation()
		{
			var factory = ObjectFactory.GetInstance<IReceiver>();
			Assert.That(factory, Is.Not.Null);
			Assert.That(factory, Is.InstanceOf<LoopbackReceiver>());
		}

		[Test]
		public void Should_get_sender_node_implementation()
		{
			var senderNode = ObjectFactory.GetInstance<ISenderNode>();
			Assert.That(senderNode, Is.InstanceOf<LoopbackSender>());
			Assert.That(senderNode, Is.Not.Null);
		}
	}
}
