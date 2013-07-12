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
		readonly IPersistentQueue _persistentQueue;
		static readonly object _lockObject = new object();
		const string Pname = "./QUEUE";

		public PersistentWorkQueue(IMessageSerialiser serialiser, IPersistentQueueFactory queueFac)
		{
			_serialiser = serialiser;

			if (!Directory.Exists(Pname)) Directory.CreateDirectory(Pname);
			_persistentQueue = queueFac.PrepareQueue(Pname);
			MakeAvailable();
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
				IfAvailable(
					DequeueItem,
				_else: 
					new WorkQueueItem<IMessage>());
		}

		WorkQueueItem<IMessage> DequeueItem()
		{
			byte[] bytes;
			lock (_lockObject)
			{
				using (var session = _persistentQueue.OpenSession())
				{
					bytes = session.Dequeue();
				}
			}
			if (bytes == null) return new WorkQueueItem<IMessage>();

			var msg = (IMessage)_serialiser.DeserialiseByStack(Encoding.UTF8.GetString(bytes));
			return new WorkQueueItem<IMessage>(
				msg, Finish, Cancel
				);
		}

		void Cancel(IMessage obj)
		{
			MakeAvailable();
		}

		void Finish(IMessage obj)
		{
			PopQueue();
			MakeAvailable();
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

		#region Available junk
		object available;
		void MakeAvailable()
		{
			Interlocked.Exchange(ref available, new object());
		}

		T IfAvailable<T>(Func<T> doThis, T _else)
		{
			var marker = Interlocked.Exchange(ref available, null);
			if (marker == null) return _else;

			return doThis();
		}
		#endregion

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

		public void Dispose()
		{
			_persistentQueue.Dispose();
		}
	}
}