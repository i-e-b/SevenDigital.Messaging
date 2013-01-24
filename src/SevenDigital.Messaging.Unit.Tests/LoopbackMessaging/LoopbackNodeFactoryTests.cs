using NUnit.Framework;
using SevenDigital.Messaging.MessageSending.Loopback;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
    [TestFixture]
    public class LoopbackNodeFactoryTests
    {
        readonly LoopbackNodeFactory subject = new LoopbackNodeFactory();

        [Test]
        public void Should_return_empty_list_if_no_listeners_are_registered_for_type()
        {
            var listeners = subject.ListenersFor<RandomType>();

            Assert.That(listeners, Is.Not.Null);
            Assert.That(listeners.Count, Is.EqualTo(0));
        }
    }

    public class RandomType
    {
         
    }
}