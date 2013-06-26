using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class ConcurrencyLimit
	{
		[Test]
		public void can_set_concurreny_limit_in_default_mode ()
		{
			MessagingSystem.Configure.WithDefaults();
			MessagingSystem.Control.SetConcurrentHandlers(1);
			MessagingSystem.Control.Shutdown();

			Assert.Pass();
		}
		
		[Test]
		public void can_set_concurreny_limit_in_loopback_mode ()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Control.SetConcurrentHandlers(1);
			MessagingSystem.Control.Shutdown();

			Assert.Pass();
		}
	}
}