using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class ReceiverTests
	{
		Receiver _subject;
		IUniqueEndpointGenerator _endpointGenerator;
		IHandlerManager _handlerManager;
		IMessageRouter _messageRouter;
		IPollingNodeFactory _pollerFactory;
		IDispatcherFactory _dispatchFactory;

		[SetUp]
		public void setup()
		{
			_endpointGenerator = Substitute.For<IUniqueEndpointGenerator>();
			_handlerManager = Substitute.For<IHandlerManager>();
			_messageRouter = Substitute.For<IMessageRouter>();
			_pollerFactory = Substitute.For<IPollingNodeFactory>();
			_dispatchFactory = Substitute.For<IDispatcherFactory>();

			_subject = new Receiver(
				_endpointGenerator,
				_handlerManager,
				_messageRouter, _pollerFactory, _dispatchFactory);
		}

		[Test]
		public void _todo()
		{
			_subject.Dispose();
			Assert.Inconclusive();
		}
	}
}