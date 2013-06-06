using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class ReceiverNodeTests
	{
		IReceiverNode _subject;
		IReceiverControl _parent;
		IRoutingEndpoint _endpoint;
		IHandlerManager _handlerManager;
		IPollingNodeFactory _pollerFactory;
		ITypedPollingNode _poller;

		[SetUp]
		public void setup()
		{
			_parent = Substitute.For<IReceiverControl>();
			_endpoint = Substitute.For<IRoutingEndpoint>();
			_handlerManager = Substitute.For<IHandlerManager>();

			_poller = Substitute.For<ITypedPollingNode>();
			
			_pollerFactory = Substitute.For<IPollingNodeFactory>();
			_pollerFactory.Create(Arg.Any<IRoutingEndpoint>()).Returns(_poller);

			_subject = new ReceiverNode(
				_parent, _endpoint, _handlerManager, _pollerFactory
				);
		}

		[Test]
		public void binding_a_message_type_to_a_handler_type_adds_the_binding_to_the_handler_manager ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_handlerManager.Received().AddHandler(typeof(IMessage), typeof(MessageHandler));
		}

		#region TypeJunk

		public class MessageHandler : IHandle<IMessage> { public void Handle(IMessage message) { } }

		#endregion TypeJunk
	}
}