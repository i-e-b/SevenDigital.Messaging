using System;
using System.Text;
using DiskQueue;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.ConfigurationActions;

namespace SevenDigital.Messaging.MessageSending.LocalQueue
{
	/// <summary>
	/// Sender node for local queues
	/// </summary>
	public sealed class LocalQueueSender : ISenderNode
	{
		readonly IMessageSerialiser _serialiser;
		readonly string _incomingPath;

		/// <summary>
		/// Create a local queue sender node
		/// <para>You should not use this yourself. Use:</para>
		/// <para>MessagingSystem.Configure.WithLocalQueue(...);</para>
		/// and send messages as normal.
		/// </summary>
		public LocalQueueSender(LocalQueueConfig config,
		                        IMessageSerialiser serialiser)
		{
			_serialiser = serialiser;
			_incomingPath = config.IncomingPath;
		}

		/// <summary>
		/// Ignored
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		public void SendMessage<T>(T message) where T : class, IMessage
		{
			var data = Encoding.UTF8.GetBytes(_serialiser.Serialise(message));

			using (var queue = PersistentQueue.WaitFor(_incomingPath, TimeSpan.FromMinutes(1)))
			using (var session = queue.OpenSession())
			{
				session.Enqueue(data);
				session.Flush();
			}
			HookHelper.TrySentHooks(message);
		}
	}
}