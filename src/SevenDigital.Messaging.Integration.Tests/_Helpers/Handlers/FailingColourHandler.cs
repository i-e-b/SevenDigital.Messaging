using System;
using System.Threading;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Handlers
{
	public class FailingColourHandler : IHandle<IColourMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public static string Message { get { return "I have failed!"; } }

		public void Handle(IColourMessage message)
		{
			AutoResetEvent.Set();
			throw new Exception(Message);
		}
	}
}