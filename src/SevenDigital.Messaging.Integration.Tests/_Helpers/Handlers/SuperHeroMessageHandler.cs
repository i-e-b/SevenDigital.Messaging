using System;
using System.Threading;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
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