using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.MessageReceiving;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Shutdown
{
	[TestFixture]
	public class ShutdownTests
	{
		[Test, Ignore("Not re-implemented yet")]
		public void Calling_messaging_shutdown_stops_dispatch_controller ()
		{
			/*var dispatchMock = Substitute.For<IDispatchController>();
			var channelMock = Substitute.For<IChannelAction>();
			ObjectFactory.Configure(map=> {
				map.For<IDispatchController>().Use(dispatchMock);
				map.For<IChannelAction>().Use(channelMock);
			});

			MessagingSystem.Control.Shutdown();

			dispatchMock.Received().Shutdown();
			channelMock.Received().Dispose();*/
		}
	}
}
