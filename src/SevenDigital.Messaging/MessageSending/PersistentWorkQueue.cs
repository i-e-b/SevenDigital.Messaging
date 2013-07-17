using System;
using System.Runtime.CompilerServices;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Persistent queue interface for dispatch sharp
	/// </summary>
	public class PersistentWorkQueue : IWorkQueue<byte[]>, IDisposable
	{
		readonly IPersistentQueue _persistentQueue;

		/// <summary>
		/// Create a new persistent queue wrapper, given a persistent queue factory.
		/// </summary>
		public PersistentWorkQueue(IPersistentQueueFactory queueFac)
		{
			_persistentQueue = queueFac.PrepareQueue();
		}

		/// <summary>
		/// Add an item to the queue 
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Enqueue(byte[] work)
		{
			using (var session = _persistentQueue.OpenSession())
			{
				session.Enqueue(work);
				session.Flush();
			}
		}


		/// <summary>
		/// Try and get an item from this queue. Success is encoded in the WQI result 'HasItem' 
		/// </summary>
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

		/// <summary>
		/// Approximate snapshot length 
		/// </summary>
		public int Length()
		{
			var pq = _persistentQueue;
			return pq == null ? 0 : pq.EstimatedCountOfItemsInQueue;
		}

		/// <summary>
		/// Advisory method: block if the queue is waiting to be populated.
		///             Should return true when items are available.
		///             Implementations may return false if polling and no items are available.
		///             Implementations are free to return immediately.
		///             Implementations are free to return true even if no items are available.
		/// </summary>
		public bool BlockUntilReady()
		{
			var pq = _persistentQueue;
			if (pq == null) return false;
			return (pq.EstimatedCountOfItemsInQueue > 0);
		}

		/// <summary>
		/// Close queue
		/// </summary>
		public void Dispose()
		{
			_persistentQueue.Dispose();
		}
	}

}