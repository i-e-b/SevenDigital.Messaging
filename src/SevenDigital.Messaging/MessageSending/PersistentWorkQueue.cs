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

		public PersistentWorkQueue(IMessageSerialiser serialiser)
		{
			_serialiser = serialiser;

			if (!Directory.Exists(Pname)) Directory.CreateDirectory(Pname);
			_persistentQueue = new PersistentQueue(Pname);
		}

		public void Enqueue(IMessage work)
		{
			byte[] raw = Encoding.UTF8.GetBytes(_serialiser.Serialise(work));
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
			lock(_lockObject)
			{
				byte[] bytes;
				using (var session = _persistentQueue.OpenSession())
				{
					bytes = session.Dequeue();
					// this means we can lose a message!
					session.Flush();
				}

				if (bytes == null) return new WorkQueueItem<IMessage>();


				var str = Encoding.UTF8.GetString(bytes);
				return new WorkQueueItem<IMessage>(
						(IMessage)_serialiser.DeserialiseByStack(str),
						m => { },
						m => { }
					);
			}
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

		public void Dispose()
		{
			_persistentQueue.Dispose();
		}
	}
}