using System;
using System.IO;
using DiskQueue;

namespace SevenDigital.Messaging.MessageSending
{
	public interface IPersistentQueueFactory
	{
		IPersistentQueue PrepareQueue();
		void Cleanup();
	}

	public class IntegrationTestQueueFactory : IPersistentQueueFactory
	{
		readonly string _storagePath;

		public IntegrationTestQueueFactory()
		{
			_storagePath =  "./QUEUE_" + (Guid.NewGuid());
			if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
		}

		public IPersistentQueue PrepareQueue()
		{
			return PersistentQueue.WaitFor(_storagePath, TimeSpan.FromSeconds(10));
		}

		public void Cleanup()
		{
			foreach (var dir in Directory.GetDirectories(".", "QUEUE_*"))
			{
				Console.WriteLine("Deleting " + dir);
				DeleteQueueFolder(dir);
			}
		}

		void DeleteQueueFolder(string path)
		{
			try
			{
				if (!Directory.Exists(path)) return;

				var files = Directory.GetFiles(_storagePath, "*", SearchOption.AllDirectories);
				Array.Sort(files, (s1, s2) => s2.Length.CompareTo(s1.Length)); // sortby length descending
				foreach (var file in files)
				{
					File.Delete(file);
				}

				Directory.Delete(path, true);
			}
			catch
			{
				Console.Write("Deleting queues failed");
			}
		}
	}

	public class PersistentQueueFactory : IPersistentQueueFactory
	{
		const string storagePath = "../../QUEUE"; // hacky, hacky, hacky :-(

		public IPersistentQueue PrepareQueue()
		{
			if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
			return PersistentQueue.WaitFor(storagePath, TimeSpan.FromSeconds(10));
		}

		public void Cleanup()
		{
			// Nothing.
		}
	}
}