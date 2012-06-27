using System.Threading;
using SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.Services;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Handlers
{
	public class SuperHeroMessageHandler : IHandle<IComicBookCharacterMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IComicBookCharacterMessage message)
		{
			AutoResetEvent.Set();
		}
	}
}