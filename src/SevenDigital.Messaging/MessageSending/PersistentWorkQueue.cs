using System;
using System.Text;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using SevenDigital.Messaging.Base.Serialisation;

namespace SevenDigital.Messaging.MessageSending
{
	public class PersistentWorkQueue : IWorkQueue<IMessage>, IDisposable
	{
		readonly IMessageSerialiser _serialiser;
		IPersistentQueue _persistentQueue;
		static readonly object _lockObject = new object();
		readonly SingleAvailable single;

		public PersistentWorkQueue(IMessageSerialiser serialiser, IPersistentQueueFactory queueFac)
		{
			_serialiser = serialiser;

			_persistentQueue = queueFac.PrepareQueue();
			single = new SingleAvailable();
			single.MakeAvailable();
		}

		public void Enqueue(IMessage work)
		{
			var raw = Encoding.UTF8.GetBytes(_serialiser.Serialise(work));
			lock (_lockObject)
			{
				if (_persistentQueue == null) return;
				using (var session = _persistentQueue.OpenSession())
				{
					session.Enqueue(raw);
					session.Flush();
				}
			}
		}

		public IWorkQueueItem<IMessage> TryDequeue()
		{
			return 
				single.IfAvailable(
					DequeueItem,
				_else: 
					new WorkQueueItem<IMessage>());
		}

		WorkQueueItem<IMessage> DequeueItem()
		{
			byte[] bytes = null;
			lock (_lockObject)
			{
				if (_persistentQueue != null)
					using (var session = _persistentQueue.OpenSession())
					{
						bytes = session.Dequeue();
					}
			}
			if (bytes == null)
			{
				single.MakeAvailable();
				return new WorkQueueItem<IMessage>();
			}

			var msg = (IMessage)_serialiser.DeserialiseByStack(Encoding.UTF8.GetString(bytes));
			return new WorkQueueItem<IMessage>(
				msg, Finish, Cancel
				);
		}

		void Cancel(IMessage obj)
		{
			single.MakeAvailable();
		}

		void Finish(IMessage obj)
		{
			PopQueue();
			single.MakeAvailable();
		}

		void PopQueue()
		{
			lock (_lockObject)
			{
				using (var session = _persistentQueue.OpenSession())
				{
					session.Dequeue();
					session.Flush();
				}
			}
		}

		public int Length()
		{
			var pq = _persistentQueue;
			return pq == null ? 0 : pq.EstimatedCountOfItemsInQueue;
		}

		public bool BlockUntilReady()
		{
			var pq = _persistentQueue;
			if (pq == null) return false;
			return (pq.EstimatedCountOfItemsInQueue > 0);
		}

		public void Dispose()
		{
			if (_persistentQueue == null) return;
			_persistentQueue.Dispose();
			_persistentQueue = null;
		}
	}

	class SingleAvailable
	{
		bool _available;
		readonly object _lock;

		public SingleAvailable()
		{
			_lock = new object();
		}

		public void MakeAvailable()
		{
			lock(_lock)
			{
				_available = true;
			}
		}

		public T IfAvailable<T>(Func<T> doThis, T _else)
		{
			bool _if;
			lock(_lock)
			{
				_if = _available;
				_available = false;
			}
			return _if ? doThis() : _else;
		}

	}
}