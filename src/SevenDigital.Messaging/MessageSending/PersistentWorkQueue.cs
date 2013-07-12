using System;
using System.IO;
using System.Text;
using System.Threading;
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
		const string Pname = "./QUEUE";
		readonly SingleAvailable single;

		public PersistentWorkQueue(IMessageSerialiser serialiser, IPersistentQueueFactory queueFac)
		{
			_serialiser = serialiser;

			if (!Directory.Exists(Pname)) Directory.CreateDirectory(Pname);
			_persistentQueue = queueFac.PrepareQueue(Pname);
			single = new SingleAvailable();
			single.MakeAvailable();
		}

		public static void DeletePendingMessages()
		{
			if (Directory.Exists(Pname))
			{
				var files = Directory.GetFiles(Pname, "*", SearchOption.AllDirectories);
				Array.Sort(files, (s1, s2) => s2.Length.CompareTo(s1.Length));// sortby length descending
				foreach (var file in files)
				{
					File.Delete(file);
				}

				Directory.Delete(Pname, true);
			}
			Directory.CreateDirectory(Pname);
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
			/*for (int i = 0; i < 10; i++)
			{
				var pq = _persistentQueue;
				if (pq == null) return false;
				if (pq.EstimatedCountOfItemsInQueue > 0) return true;
				Thread.Sleep(100);
			}
			return false;*/
			return true;
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
		object _available;

		public void MakeAvailable()
		{
			Interlocked.Exchange(ref _available, new object());
		}

		public T IfAvailable<T>(Func<T> doThis, T _else)
		{
			var marker = Interlocked.Exchange(ref _available, null);
			return marker != null ? doThis() : _else;
		}

	}
}