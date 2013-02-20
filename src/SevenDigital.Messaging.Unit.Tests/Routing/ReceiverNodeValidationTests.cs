using System;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using SevenDigital.Messaging.Unit.Tests._Helpers;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Routing
{
	[TestFixture]
	public class ReceiverNodeValidationTests
	{
		[Test]
		public void Should_throw_exception_if_Handle_is_called_with_non_interface_type ()
		{
            if (Iam.RunningMono()) Assert.Inconclusive("This test doesn't run under Mono");
			// Mono has a reflection fault that stop CastleProxy faking correctly

			ObjectFactory.Configure(map=>map.For<INode>().Use(Substitute.For<INode>()));
			var subject = new ReceiverNode(Substitute.For<IRoutingEndpoint>());

			var exception = Assert.Throws<ArgumentException>(() =>  subject.Handle<Instance>());
			Assert.That(exception.Message, Is.EqualTo("Handler type must be an interface that implements IMessage"));
		}

		abstract class Instance : IMessage
		{
			public Guid CorrelationId { get; set; }
		}
	}
}
