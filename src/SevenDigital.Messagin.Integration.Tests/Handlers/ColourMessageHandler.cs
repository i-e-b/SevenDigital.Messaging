using System.Threading;
using SevenDigital.Messaging.Integration.Tests.Messages;
using SevenDigital.Messaging.Services;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
{
	public class ColourMessageHandler : IHandle<IColourMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IColourMessage message)
		{
			AutoResetEvent.Set();
		}
	}
}