using System;
using System.IO;
using System.Text;
using DiskQueue;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	/// <summary>
	/// An event hook that writes handler exceptions from this
	/// process to a locally accessible DiskQueue.
	/// <para>If you manually add this queue, you must
	/// add a ctor storage parameter to the structure map configuration.</para>
	/// </summary>
	public class LocalQueueExceptionHook : IEventHook
	{
		readonly IMessageSerialiser _serialiser;
		readonly string _errorQueueStorage;

		/// <summary>
		/// Create a new event hook, with the given storage queue.
		/// <para>You should use the static <see cref="Inject"/> method to
		/// add this hook.</para>
		/// </summary>
		public LocalQueueExceptionHook(IMessageSerialiser serialiser, string errorQueuePath)
		{
			_serialiser = serialiser;
			_errorQueueStorage = Path.Combine(errorQueuePath, SDM_Configure.IncomingQueueSubpath);
		}

		/// <summary>
		/// Inject this hook into the messaging system.
		/// Any handler exceptions will be written to the queue
		/// as `<see cref="IHandlerExceptionMessage"/>`s
		/// </summary>
		public static void Inject(string errorQueuePath)
		{
			ObjectFactory.Configure(map => map
				.For<IEventHook>()
				.Add<LocalQueueExceptionHook>()
				.Ctor<string>().Is(errorQueuePath));
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public void MessageSent(IMessage message) { }

		/// <summary>
		/// Does nothing
		/// </summary>
		public void MessageReceived(IMessage message) { }

		/// <summary>
		/// A message was received by this process, but a handler threw an exception
		/// </summary>
		/// <param name="message">The incoming message</param>
		/// <param name="handler">Type of the failed handler</param>
		/// <param name="error">Exception thrown</param>
		public void HandlerFailed(IMessage message, Type handler, Exception error)
		{
			try
			{
				var failureMessage = new HandlerExceptionMessage
				{
					CausingMessage = message,
					Date = DateTime.UtcNow,
					Exception = error.GetType() + ": " + error.Message,
					HandlerTypeName = handler.FullName
				};

				var data = Encoding.UTF8.GetBytes(_serialiser.Serialise(failureMessage));

				using (var queue = PersistentQueue.WaitFor(_errorQueueStorage, TimeSpan.FromMinutes(1)))
				using (var session = queue.OpenSession())
				{
					session.Enqueue(data);
					session.Flush();
				}
			}
			catch (Exception ex)
			{
				Log.Warning("Could not write handler failure to local queue (" + ex.GetType() + ") " + ex.Message);
			}
		}
	}

	class HandlerExceptionMessage : IHandlerExceptionMessage
	{
		public HandlerExceptionMessage()
		{
			CorrelationId = Guid.NewGuid();
		}

		public Guid CorrelationId { get; set; }
		public DateTime Date { get; set; }
		public IMessage CausingMessage { get; set; }
		public string Exception { get; set; }
		public string HandlerTypeName { get; set; }
	}
}