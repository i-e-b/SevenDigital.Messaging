using System.Threading;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.Services;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
{
	public class VillainMessageHandler : IHandle<IComicBookCharacterMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IComicBookCharacterMessage message)
		{
			AutoResetEvent.Set();
		}
	}
}