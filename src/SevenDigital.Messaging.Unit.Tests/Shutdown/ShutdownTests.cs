using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.RabbitMq;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Shutdown
{
	[TestFixture]
	public class ShutdownTests
	{
		IReceiverControl _receiverControl;
		IChannelAction _channelAction;
		ISenderNode _sender;
		IRabbitMqConnection _rabbitConn;

		[SetUp]
		public void a_set_of_configured_components_being_shut_down()
		{
			MessagingSystem.Control.Shutdown();

			_receiverControl = Substitute.For<IReceiverControl>();
			_channelAction = Substitute.For<IChannelAction>();
			_sender = Substitute.For<ISenderNode>();
			_rabbitConn = Substitute.For<IRabbitMqConnection>();

			ObjectFactory.Configure(map =>
			{
				map.For<IReceiverControl>().Use(_receiverControl);
				map.For<IChannelAction>().Use(_channelAction);
				map.For<ISenderNode>().Use(_sender);
				map.For<IRabbitMqConnection>().Use(_rabbitConn);
			});

			MessagingSystem.Control.Shutdown();
		}

		[Test]
		public void should_dispose_of_channel_action()
		{
			_channelAction.Received().Dispose();
		}
		[Test]
		public void should_dispose_of_sender_node()
		{
			_sender.Received().Dispose();
		}
		[Test]
		public void should_dispose_of_rabbit_mq_connection()
		{
			_rabbitConn.Received().Dispose();
		}
		[Test]
		public void should_dispose_of_receiver_node()
		{
			_receiverControl.Received().Dispose();
		}
	}
}