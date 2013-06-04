using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
	[TestFixture]
	public class ShutdownTests
	{
		[SetUp]
		public void a_shutdown_messaging_system ()
		{
			MessagingSystem.Configure.WithDefaults();
			MessagingSystem.Control.Shutdown();
		}

		[Test]
		public void should_have_ejected_configuration ()
		{
			Assert.That(ObjectFactory.GetAllInstances<IReceiver>(), Is.Empty);
		}
	}
}