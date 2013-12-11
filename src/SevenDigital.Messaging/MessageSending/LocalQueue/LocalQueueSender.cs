using System;
using System.Text;
using DiskQueue;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.ConfigurationActions;

namespace SevenDigital.Messaging.MessageSending.LocalQueue
{
	public class LocalQueueSender : ISenderNode
	{
		readonly IMessageSerialiser _serialiser;
		readonly string _incomingPath;

		public LocalQueueSender(LocalQueueConfig config,
		                        IMessageSerialiser serialiser)
		{
			_serialiser = serialiser;
			_incomingPath = config.IncomingPath;
		}

		public void Dispose() { }

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