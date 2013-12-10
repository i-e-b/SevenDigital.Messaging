using System.Threading;
using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests.Api
{
	[TestFixture]
	public class AutoShutdownTests
	{
		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}


		[Test/*, Repeat(10)*/]
		public void if_the_thread_messaging_is_configured_on_ends_messaging_gets_shut_down ()
		{
			var t = new Thread(() => {
				MessagingSystem.Configure.WithDefaults();
				Thread.Sleep(100);
			})
			{
				Name = "Test setup thread"
			};
			t.Start();
			t.Join();
			Thread.Sleep(500);

			Assert.That(MessagingIsStillRunning(), Is.False);
		}

		[Test]
		public void if_any_other_thread_ends_the_messaging_system_is_NOT_shut_down ()
		{
			MessagingSystem.Configure.WithDefaults();

			var t = new Thread(()=> Thread.Sleep(10));
			t.Start();
			t.Join();

			Assert.That(MessagingIsStillRunning());
		}

		static bool MessagingIsStillRunning()
		{
			try
			{
				MessagingSystem.Sender();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}