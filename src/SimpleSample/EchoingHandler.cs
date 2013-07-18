using System;
using SevenDigital.Messaging;

namespace SimpleSample
{
	public class EchoingHandler : IHandle<IEchoMessage>
	{
		public void Handle(IEchoMessage message)
		{
			Console.Write("# Recevied \"" + message.Message);
			Console.WriteLine("\" (" + message.GetType() + ")");
		}
	}
}