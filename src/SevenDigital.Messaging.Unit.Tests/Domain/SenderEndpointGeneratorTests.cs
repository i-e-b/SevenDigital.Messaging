using NUnit.Framework;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.Domain
{
	[TestFixture]
	public class SenderEndpointGeneratorTests
	{
		[Test]
		public void Sender_endpoint_name_should_always_be_Sender ()
		{
			var x= new SenderEndpointGenerator();
			Assert.That(x.Generate().ToString(), Is.EqualTo("Sender"));
		}
	}
}
