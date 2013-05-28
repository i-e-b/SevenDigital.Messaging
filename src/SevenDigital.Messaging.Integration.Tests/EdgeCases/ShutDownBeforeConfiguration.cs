using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests.EdgeCases
{
	[TestFixture]
	public class ShutDownBeforeConfiguration
	{
		[Test]
		public void can_call_shutdown_without_having_configured_messaging()
		{
			Messaging.Control.Shutdown();
			Assert.Pass();
		}
	}
}
