using System.Text;
using System.Threading;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using SevenDigital.Messaging.Base.Serialisation;

namespace SevenDigital.Messaging.MessageSending
{
	public class PersistentWorkQueue : IWorkQueue<IMessage>
	{
		readonly IMessageSerialiser _serialiser;
		readonly IPersistentQueue _persistentQueue;

		public PersistentWorkQueue(IMessageSerialiser serialiser)
		{
			_serialiser = serialiser;
			_persistentQueue = new PersistentQueue("./QUEUE");
		}

		public void Enqueue(IMessage work)
		{
			byte[] raw = Encoding.UTF8.GetBytes(_serialiser.Serialise(work));
			using (var session = _persistentQueue.OpenSession())
			{
				session.Enqueue(raw);
			}
		}

		public IWorkQueueItem<IMessage> TryDequeue()
		{
			var floatingSession = _persistentQueue.OpenSession();
			var bytes = floatingSession.Dequeue();
			if (bytes == null) return new WorkQueueItem<IMessage>();
			var str = Encoding.UTF8.GetString(bytes);

			return new WorkQueueItem<IMessage>(
				(IMessage)_serialiser.DeserialiseByStack(str),
				m => {
					floatingSession.Flush();
					floatingSession.Dispose();
				},
				m => floatingSession.Dispose()
				);
		}

		public int Length()
		{
			return _persistentQueue.EstimatedCountOfItemsInQueue;
		}

		public bool BlockUntilReady()
		{
			for (int i = 0; i < 10; i++)
			{
				if (_persistentQueue.EstimatedCountOfItemsInQueue > 0) return true;
				Thread.Sleep(100);
			}
			return false;
		}
	}
}