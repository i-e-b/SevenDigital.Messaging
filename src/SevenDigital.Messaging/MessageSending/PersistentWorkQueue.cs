using System;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;

namespace SevenDigital.Messaging.MessageSending
{
	public class PersistentWorkQueue : IWorkQueue<byte[]>, IDisposable
	{
		IPersistentQueue _persistentQueue;
		static readonly object _lockObject = new object();
		readonly SingleAvailable single;

		public PersistentWorkQueue(IPersistentQueueFactory queueFac)
		{
			_persistentQueue = queueFac.PrepareQueue();
			single = new SingleAvailable();
			single.MakeAvailable();
		}

		public void Enqueue(byte[] work)
		{
			lock (_lockObject)
			{
				if (_persistentQueue == null) return;
				using (var session = _persistentQueue.OpenSession())
				{
					session.Enqueue(work);
					session.Flush();
				}
			}
		}

		public IWorkQueueItem<byte[]> TryDequeue()
		{
			return 
				single.IfAvailable(
					DequeueItem,
				_else: 
					new WorkQueueItem<byte[]>());
		}

		WorkQueueItem<byte[]> DequeueItem()
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
				return new WorkQueueItem<byte[]>();
			}

			return new WorkQueueItem<byte[]>(
				bytes, Finish, Cancel
				);
		}

		void Cancel(byte[] obj)
		{
			single.MakeAvailable();
		}

		void Finish(byte[] obj)
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