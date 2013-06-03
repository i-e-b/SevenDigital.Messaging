using System;
using System.Threading;
using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
{
	public class ColourMessageHandler : IHandle<IColourMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IColourMessage message)
		{
			Console.WriteLine("Got: " + message.GetType());
			AutoResetEvent.Set();
		}
	}

	public class AnotherColourMessageHandler : IHandle<IColourMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public void Handle(IColourMessage message)
		{
			AutoResetEvent.Set();
		}
	}

	public class TwoColourMessageHandler : IHandle<ITwoColoursMessage>
	{
		public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		public static int ReceivedCount = 0;
		public static ITwoColoursMessage ReceivedMessage { get; private set; }

		public static void Prepare()
		{
			AutoResetEvent = new AutoResetEvent(false);
			ReceivedCount = 0;
		}

		public void Handle(ITwoColoursMessage message)
		{
			Interlocked.Increment(ref ReceivedCount);
			ReceivedMessage = message;
			AutoResetEvent.Set();
		}
	}

	public class AllColourMessagesHandler : IHandle<ITwoColoursMessage>, IHandle<IColourMessage>
	{
		public static AutoResetEvent AutoResetEventForColourMessage = new AutoResetEvent(false);
		public static AutoResetEvent AutoResetEventForTwoColourMessage = new AutoResetEvent(false);

		public static void Prepare()
		{
			AutoResetEventForColourMessage = new AutoResetEvent(false);
			AutoResetEventForTwoColourMessage = new AutoResetEvent(false);
		}

		public void Handle(ITwoColoursMessage message)
		{
			AutoResetEventForTwoColourMessage.Set();
		}

		public void Handle(IColourMessage message)
		{
			AutoResetEventForColourMessage.Set();
		}
	}
}