using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Loopback
{
	[TestFixture]
	public class LoopbackModeTests
	{
		[Test]
		public void Loopback_mode_configures_correctly ()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			
			using (var listener = MessagingSystem.Receiver().Listen())
			{
				listener.Handle<IMessage>().With<IntegrationHandler>();
			}
			
			ObjectFactory.EjectAllInstancesOf<IReceiver>();
			//ObjectFactory.EjectAllInstancesOf<INode>();
			ObjectFactory.EjectAllInstancesOf<ITestEventHook>();
		}
	}

	public class IntegrationHandler:IHandle<IMessage>
	{
		public void Handle(IMessage message)
		{
		}
	}
}
