using System;
using NUnit.Framework;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.Routing
{
	[TestFixture]
	public class SenderEndpointGeneratorTests
	{
		[Test]
		public void Sender_endpoint_name_should_always_be_Sender ()
		{
			var endpoint= new SenderEndpointGenerator().Generate();

			Console.WriteLine(endpoint);

			Assert.That(endpoint.ToString(), Contains.Substring("Sender"));
			Assert.That(endpoint.ToString(), Contains.Substring(Environment.MachineName));
		}
	}
}
