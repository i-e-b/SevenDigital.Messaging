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
			Messaging.Configure.WithLoopbackMode();

			Messaging.Receiver().Listen().Handle<IMessage>().With<AHandler>();
		}

		[Test]
		public void Should_not_throw_exception_if_in_loopback_mode()
		{
			Assert.DoesNotThrow(() => Messaging.Testing.LoopbackListenersForMessage<IMessage>());
		}
	}

	public class AHandler : IHandle<IMessage>
	{
		public void Handle(IMessage message)
		{

		}
	}
}
