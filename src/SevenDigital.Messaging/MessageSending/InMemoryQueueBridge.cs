using System.Collections.Generic;
using DiskQueue;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Simulates a PersistentQueue, but only in memory
	/// </summary>
	public sealed class InMemoryQueueBridge : IPersistentQueue
	{
		readonly InnerQueue _innerQueue;

		/// <summary>
		/// Create a new memory bridge
		/// </summary>
		public InMemoryQueueBridge()
		{
			_innerQueue = new InnerQueue();
		}

		/// <summary>
		/// Nasty fake version of persistent queue behaviour
		/// </summary>
		class InnerQueue: IPersistentQueueSession
		{
			public readonly Queue<byte[]> Queue;
			static readonly object _lock = new object();

			public InnerQueue()
			{
				Queue = new Queue<byte[]>();
			}

			public void Enqueue(byte[] data)
			{
				lock(_lock)
				{
					Queue.Enqueue(data);
				}
			}

			public byte[] Dequeue()
			{
				lock (_lock)
				{
					return Queue.Count < 1 ? null : Queue.Dequeue();
				}
			}

			public void Dispose() {}
			public void Flush(){}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose(){if (_innerQueue != null) _innerQueue.Dispose();}

		/// <summary>
		/// Start a session
		/// </summary>
		/// <returns></returns>
		public IPersistentQueueSession OpenSession()
		{
			return _innerQueue;
		}

		/// <summary>
		/// Count
		/// </summary>
		public int EstimatedCountOfItemsInQueue { get { return _innerQueue.Queue.Count;}}

		/// <summary>
		/// Nothing here. Don't use.
		/// </summary>
		public IPersistentQueueImpl Internals { get { return null; }}

		/// <summary>
		/// Nothing here. Don't use
		/// </summary>
		public int MaxFileSize { get { return 1000; } }
	}
}