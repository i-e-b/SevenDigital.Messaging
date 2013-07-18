using System;
using System.IO;
using System.Text;
using SevenDigital.Messaging;

namespace SimpleSample
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Type lines to echo across messaging");
			Console.WriteLine("Type a blank line to exit");

			MessagingSystem.Configure.WithDefaults();

			MessagingSystem.Receiver().Listen(_=>_
				.Handle<IEchoMessage>().With<EchoingHandler>());

			try
			{
				while (true)
				{
					var line = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(line)) break;

					Console.WriteLine("Sending " + line);
					MessagingSystem.Sender().SendMessage(new EchoMessage(line));
				}
			} catch (Exception ex)
			{
				Console.WriteLine(ex.GetType() + "; " + ex.Message);
			}

			Console.WriteLine("Done. Shutting down...");
			MessagingSystem.Control.Shutdown();
		}
	}
	public class KeywordWatcherStreamWrapper : TextWriter
	{
		private readonly TextWriter underlyingStream;
		private readonly string keyword;
		public event EventHandler KeywordFound;

		public KeywordWatcherStreamWrapper(TextWriter underlyingStream, string keyword)
		{
			this.underlyingStream = underlyingStream;
			this.keyword = keyword;
		}

		public override Encoding Encoding
		{
			get { return underlyingStream.Encoding; }
		}

		public override void Write(char value)
		{
			if (value == 'x') Console.WriteLine("?");
			else underlyingStream.Write(value);
		}

		public override void Write(string s)
		{
			underlyingStream.Write(s);
			if (s.Contains(keyword))
				if (KeywordFound != null)
					KeywordFound(this, EventArgs.Empty);
		}

		public override void WriteLine(string s)
		{
			underlyingStream.WriteLine(s);
			if (s.Contains(keyword))
				if (KeywordFound != null)
					KeywordFound(this, EventArgs.Empty);
		}
	}

	public class EchoingHandler : IHandle<IEchoMessage>
	{
		public void Handle(IEchoMessage message)
		{
			Console.WriteLine("# Recevied: " + message.Message);
		}
	}

	public interface IEchoMessage:IMessage
	{
		string Message { get; set; }
	}

	public class EchoMessage : IEchoMessage
	{
		public EchoMessage(string msg)
		{
			CorrelationId = Guid.NewGuid();
			Message = msg;
		}
		public Guid CorrelationId { get; set; }
		public string Message { get; set; }
	}
}
