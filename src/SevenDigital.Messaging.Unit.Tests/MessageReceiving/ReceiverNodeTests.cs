using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
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
			_endpoint.ToString().Returns("endpoint");
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

		[Test]
		public void binding_a_message_type_to_a_handler_type_adds_the_type_to_the_polling_node ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_poller.Received().AddMessageType(typeof(IMessage));
		}

		[Test]
		public void unbinding_a_message_type_removes_the_handler_associations ()
		{
			_subject.Handle<IMessage>().With<MessageHandler>();
			_subject.Unregister<MessageHandler>();

			_handlerManager.RemoveHandler(typeof(MessageHandler));
		}

		[Test]
		public void disposing_of_the_node_unregisters_the_node_from_parent ()
		{
			_subject.Dispose();
			_parent.Received().Remove(_subject);
		}

		[Test]
		public void destination_name_comes_from_endpoint ()
		{
			Assert.That(_subject.DestinationName, Is.EqualTo("endpoint"));
			_endpoint.Received().ToString();
		}

		#region TypeJunk

		public class MessageHandler : IHandle<IMessage> { public void Handle(IMessage message) { } }

		#endregion TypeJunk
	}
}