using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Persistent queue interface for dispatch sharp
	/// </summary>
	public sealed class PersistentWorkQueue : IWorkQueue<byte[]>, IDisposable
	{
		readonly IPersistentQueue _persistentQueue;
		readonly ISleepWrapper _sleeper;

		/// <summary>
		/// Create a new persistent queue wrapper, given a persistent queue factory.
		/// </summary>
		public PersistentWorkQueue(IOutgoingQueueFactory queueFac, ISleepWrapper sleeper)
		{
			_sleeper = sleeper;
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

		int inFlight;

		/// <summary>
		/// Try and get an item from this queue. Success is encoded in the WQI result 'HasItem' 
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IWorkQueueItem<byte[]> TryDequeue()
		{
			Interlocked.Increment(ref inFlight);
			if (inFlight > 1)
			{
				Interlocked.Decrement(ref inFlight);
				return new WorkQueueItem<byte[]>();
			}

			var bytes = NextItem();

			if (bytes == null)
			{
				Interlocked.Decrement(ref inFlight);
				return new WorkQueueItem<byte[]>();
			}

			return new WorkQueueItem<byte[]>(
				bytes,
				finish =>
				{
					Pop();
					Interlocked.Decrement(ref inFlight);
				},
				cancel => Interlocked.Decrement(ref inFlight));
		}

		/// <summary>
		/// Remove front item from queue
		/// </summary>
		void Pop()
		{
			using (var session = _persistentQueue.OpenSession())
			{
				session.Dequeue();
				session.Flush();
			}
		}

		/// <summary>
		/// Peek front item in queue.
		/// </summary>
		byte[] NextItem()
		{
			using (var session = _persistentQueue.OpenSession())
			{
				return session.Dequeue();
			}
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

			if (pq.EstimatedCountOfItemsInQueue <= 0)
			{
				_sleeper.SleepMore();
				return false;
			}
			_sleeper.Reset();
			return true;
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