using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
    [TestFixture]
    public class LoopbackRegistrationTests
    {
        [SetUp]
        public void SetUp()
        {
            new MessagingConfiguration().WithLoopback();

            ObjectFactory.GetInstance<INodeFactory>().Listen().Handle<IMessage>().With<AHandler>();
        }

        [Test]
        public void Should_not_throw_exception_if_in_loopback_mode()
        {
            Assert.DoesNotThrow(() => new MessagingConfiguration().LoopbackListenersForMessage<IMessage>());
        }
    }

    public class AHandler : IHandle<IMessage>
    {
        public void Handle(IMessage message)
        {

        }
    }
}
