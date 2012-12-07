using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.Shutdown
{
	[TestFixture]
	public class ShutdownTests
	{
		[Test]
		public void Calling_messaging_shutdown_stops_dispatch_controller ()
		{
			var mock = Substitute.For<IDispatchController>();
			ObjectFactory.Configure(map=>map.For<IDispatchController>().Use(mock));

			new MessagingConfiguration().Shutdown();

			mock.Received().Shutdown();
		}
	}
}
