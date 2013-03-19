using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Unit.Tests._Helpers;

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
            if (Iam.RunningMono()) Assert.Inconclusive("This test doesn't run under Mono");
			// Mono has a reflection fault that stop CastleProxy faking correctly

			dispatch = Substitute.For<IDispatchController>();
			poller = Substitute.For<IDestinationPoller>();
			dispatch.CreatePoller("").ReturnsForAnyArgs(poller);

			using(var subject = new Node(null, dispatch))
			{
				subject.SetEndpoint(new Endpoint("x"));
			}

			poller.Received().Stop();

		}
	}
}
