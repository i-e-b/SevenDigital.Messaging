using System;
using SevenDigital.Messaging;

namespace SimpleSample
{
	public class Program
	{
		static void Main(string[] args)
		{
			AskUserAndConfigureMessaging();

			Console.WriteLine("Type lines to echo across messaging");
			Console.WriteLine("Type a blank line to exit");

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

		static void AskUserAndConfigureMessaging()
		{
			Console.WriteLine("Pick a messaging mode:");
			Console.WriteLine("  1  Local host RabbitMQ broker (default)");
			Console.WriteLine("  2  Loopback mode");
			Console.WriteLine("  3  Local disk queue storage");
			var selection = Console.ReadKey(true);
			switch (selection.KeyChar)
			{
				case '2':
					{
						Console.WriteLine("Using loopback mode");
						MessagingSystem.Configure.WithLoopbackMode();
					}
					return;

				case '3':
					{
						Console.WriteLine("Using local disk queue storage");
						MessagingSystem.Configure.WithLocalQueue("./simpleSampleQueue");
					}
					return;

				default:
					{
						Console.WriteLine("Using localhost RabbitMQ");
						MessagingSystem.Configure.WithDefaults();
					}
					return;
			}
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
