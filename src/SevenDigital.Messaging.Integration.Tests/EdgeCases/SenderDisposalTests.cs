using System.Diagnostics;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests._Helpers;

namespace SevenDigital.Messaging.Integration.Tests.EdgeCases
{
	[TestFixture]
	public class SenderDisposalTests
	{
		[Test]
		public void non_persistent_sender_node_shuts_down_correctly ()
		{
			var sw = new Stopwatch();
			
			Helper.SetupTestMessagingNonPersistent();

			var sender = MessagingSystem.Sender();

			sw.Start();
			sender.Dispose();
			sw.Stop();
			MessagingSystem.Control.Shutdown();

			Assert.That(sw.Elapsed.TotalSeconds, Is.LessThanOrEqualTo(1));
		}
		
		[Test]
		public void default_sender_node_shuts_down_correctly ()
		{
			var sw = new Stopwatch();
			
			Helper.SetupTestMessaging();

			var sender = MessagingSystem.Sender();

			sw.Start();
			sender.Dispose();
			sw.Stop();
			MessagingSystem.Control.Shutdown();

			Assert.That(sw.Elapsed.TotalSeconds, Is.LessThanOrEqualTo(1));
		}
	}
}