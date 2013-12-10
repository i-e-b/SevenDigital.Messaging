using System.Threading;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Handlers
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