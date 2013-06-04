using NUnit.Framework;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Configuration
{
	[TestFixture]
	public class IntegrationTestMode
	{
		[SetUp]
		public void setup()
		{
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();
		}

		[Test]
		public void should_use_integration_test_endpoint_generator ()
		{
			Assert.That(
				ObjectFactory.GetInstance<IUniqueEndpointGenerator>(),
				Is.InstanceOf<IntegrationEndpointGenerator>()
				);
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}

	}
}