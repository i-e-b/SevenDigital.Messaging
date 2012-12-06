using System;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class UnreachableEndpointTests
	{
		[SetUp]
		public void Messaging_configured_to_point_at_an_unreachable_server ()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer("example.com");
		}

		[Test, Explicit]
		public void Sending_message_should_try_for_at_least_60_seconds ()
		{
			var sender = ObjectFactory.GetInstance<ISenderNode>();
			var start = DateTime.Now;

			try
			{
				sender.SendMessage(new GreenMessage());
			}
			catch
			{
				Console.WriteLine("Final fail");
			}

			var time = DateTime.Now - start;
			Assert.That(time.TotalSeconds, Is.GreaterThan(60));
		}
	}
}
