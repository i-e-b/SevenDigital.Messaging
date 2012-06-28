using NUnit.Framework;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.Routing
{
	[TestFixture]
	public class UniqueEndpointGeneratorTests
	{
		[Test]
		public void Should_generate_a_unique_endpoint()
		{
			var uniqueEndpointGenerator = new UniqueEndpointGenerator();
			var endpointA = uniqueEndpointGenerator.Generate();
			var endpointB = uniqueEndpointGenerator.Generate();
			var endpointC = uniqueEndpointGenerator.Generate();

			Assert.That(endpointA, Is.Not.EqualTo(endpointB));
			Assert.That(endpointA, Is.Not.EqualTo(endpointC));
			Assert.That(endpointB, Is.Not.EqualTo(endpointC));
		}
	}
}