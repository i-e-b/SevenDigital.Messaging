using System;
using NUnit.Framework;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.Routing
{
	[TestFixture]
	public class UniqueEndpointGeneratorTests
	{
		[Test]
		public void Should_generate_a_name_based_on_hostname_and_running_assembly_and_a_hash()
		{
			// This one is a bugger to test, but the implemented behaviour is generally helpful!
			var uniqueEndpointGenerator = new UniqueEndpointGenerator();
			var endpoint = uniqueEndpointGenerator.Generate();

			Assert.That(endpoint.ToString(), Contains.Substring(Naming.MachineName()));
			Assert.That(endpoint.ToString().Length,Is.GreaterThan(Environment.MachineName.Length));
		}
	}
}