using System;
using SevenDigital.Messaging;

namespace SimpleSample
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Type lines to echo across messaging");
			Console.WriteLine("Type a blank line to exit");

			// Hook up to "localhost" RabbitMQ
			MessagingSystem.Configure.WithDefaults();

			// Route incoming messages
			MessagingSystem.Receiver().Listen(_=>_
				.Handle<IEchoMessage>().With<EchoingHandler>());

			while (true)
			{
				try
				{
					var line = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(line)) break;

					MessagingSystem.Sender().SendMessage(new EchoMessage(line));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.GetType() + "; " + ex.Message);
				}
			}

			Console.WriteLine("Done. Shutting down...");
			MessagingSystem.Control.Shutdown();
		}
	}

	/// <summary>
	/// Message type that holds a simple string
	/// </summary>
	public interface IEchoMessage : IMessage
	{
		string Message { get; set; }
	}

	/// <summary>
	/// Concrete message to send.
	/// </summary>
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
