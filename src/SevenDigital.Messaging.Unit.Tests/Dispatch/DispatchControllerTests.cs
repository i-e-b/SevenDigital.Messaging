using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class DispatchControllerTests
	{
		IDispatchController subject;
		volatile int destinationPollerCalls;

		[SetUp]
		public void A_dispatch_controller ()
		{
			destinationPollerCalls = 0;
			ObjectFactory.Configure(map => map.For<IDestinationPoller>().Use(() => { 
				destinationPollerCalls++;
				return Substitute.For<IDestinationPoller>(); }));

			subject = new DispatchController();
		}

		[Test]
		public void When_creating_a_poller_should_get_it_from_object_factory ()
		{
			var result = subject.CreatePoller("name");

			Assert.That(destinationPollerCalls, Is.EqualTo(1));
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void Should_create_a_new_poller_for_ever_call_to_create ()
		{
			subject.CreatePoller("name");
			var result = subject.CreatePoller("name");

			Assert.That(destinationPollerCalls, Is.EqualTo(2));
			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void When_creating_a_poller_should_set_the_destination_name ()
		{
			var result = subject.CreatePoller("name");

			result.Received().SetDestinationToWatch("name");
		}

		[Test]
		public void When_stopping_dispatch_controller_should_stop_all_pollers ()
		{
			var one = subject.CreatePoller("name");
			var two = subject.CreatePoller("name");
			var three = subject.CreatePoller("name");

			subject.Shutdown();

			one.Received().Stop();
			two.Received().Stop();
			three.Received().Stop();
		}
	}
}
