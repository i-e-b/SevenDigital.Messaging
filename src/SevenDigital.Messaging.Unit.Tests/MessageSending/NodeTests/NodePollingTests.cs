using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending.NodeTests
{
	[TestFixture]
	public class NodePollingTests
	{
		IDispatchController dispatch;
		IDestinationPoller poller;

		[Test]
		public void Disposing_a_node_should_call_stop_on_its_poller ()
		{
			dispatch = Substitute.For<IDispatchController>();
			poller = Substitute.For<IDestinationPoller>();
			dispatch.CreatePoller("").ReturnsForAnyArgs(poller);

			using(var subject = new Node(null, null, dispatch))
			{
				subject.SetEndpoint(new Endpoint("x"));
			}

			poller.Received().Stop();

		}
	}
}
