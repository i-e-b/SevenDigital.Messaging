using Moq;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class DispatcherDisposalTest
	{
		Mock<IDestinationPoller> poller;

		[Test]
		public void When_disposing_of_dispatcher_should_stop_polling_thread ()
		{
			poller = new Mock<IDestinationPoller>();

			using (new DispatchInterface(null, poller.Object, null))
			{
			}

			poller.Verify(m=>m.Stop());
		}
	}
}
