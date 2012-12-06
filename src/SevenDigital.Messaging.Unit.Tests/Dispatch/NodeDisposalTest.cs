using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class NodeDisposalTest
	{
		Mock<IDestinationPoller> poller;

		[Test]
		public void When_disposing_of_dispatcher_should_stop_polling_thread ()
		{
			poller = new Mock<IDestinationPoller>();

			using (new Node(null, null, poller.Object))
			{
			}

			poller.Verify(m=>m.Stop());
		}
	}
}
