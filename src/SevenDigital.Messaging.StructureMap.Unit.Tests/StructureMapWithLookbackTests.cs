using NUnit.Framework;
using SevenDigital.Messaging.MessageSending.Loopback;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class StructureMapWithLookbackTests
	{
		const string HostName = "my.unique.host";

		[TestFixtureSetUp]
		public void Setup()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer(HostName);
            new MessagingConfiguration().WithLoopback();
		}

        [TestFixtureTearDown]
        public void TearDown()
        {
            ObjectFactory.Container.Dispose();
        }

		[Test]
		public void Should_get_node_factory_implementation()
		{
			var factory = ObjectFactory.GetInstance<INodeFactory>();
			Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.InstanceOf<LoopbackNodeFactory>());
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
