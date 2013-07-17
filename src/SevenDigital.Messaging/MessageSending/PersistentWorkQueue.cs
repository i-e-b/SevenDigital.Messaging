using System;
using System.Runtime.CompilerServices;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;

namespace SevenDigital.Messaging.MessageSending
{
	public class PersistentWorkQueue : IWorkQueue<byte[]>, IDisposable
	{
		readonly IPersistentQueue _persistentQueue;

		public PersistentWorkQueue(IPersistentQueueFactory queueFac)
		{
			_persistentQueue = queueFac.PrepareQueue();
		}
		
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Enqueue(byte[] work)
		{
			using (var session = _persistentQueue.OpenSession())
			{
				session.Enqueue(work);
				session.Flush();
			}
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public IWorkQueueItem<byte[]> TryDequeue()
		{
			var session = _persistentQueue.OpenSession();
			var bytes = session.Dequeue();

			if (bytes == null)
			{
				session.Dispose();
				return new WorkQueueItem<byte[]>();
			}

			return new WorkQueueItem<byte[]>(
				bytes, 
				finish => {
					session.Flush();
					session.Dispose();
				}, 
				cancel => session.Dispose());
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
			_persistentQueue.Dispose();
		}
	}

}