using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
	[TestFixture]
	public class LoopbackStabilityTests
	{
		[Test]
		public void clearing_event_hooks_after_settings_up_loopback_should_leave_loopback_hooks_in_place ()
		{
			MessagingSystem.Configure.WithLoopbackMode();

			MessagingSystem.Events.ClearEventHooks();

			Assert.That(
				ObjectFactory.GetInstance<IEventHook>(),
				Is.InstanceOf<TestEventHook>());
		}
	}
}
