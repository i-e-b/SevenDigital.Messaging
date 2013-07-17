using System;
using System.IO;
using DiskQueue;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Contract for persistent queue factory
	/// </summary>
	public interface IPersistentQueueFactory
	{
		/// <summary>
		/// Ensure queue exists on disk and return a locked instance
		/// </summary>
		IPersistentQueue PrepareQueue();

		/// <summary>
		/// Do any cleanup after queue is disposed.
		/// </summary>
		void Cleanup();
	}

	/// <summary>
	/// Queue factory for integration testing.
	/// Creates unique queue per request and deletes on cleanup.
	/// </summary>
	public class IntegrationTestQueueFactory : IPersistentQueueFactory
	{
		readonly string _storagePath;

		/// <summary>
		/// Create a new integration test queue
		/// </summary>
		public IntegrationTestQueueFactory()
		{
			_storagePath =  "./QUEUE_" + (Guid.NewGuid());
			if (!Directory.Exists(_storagePath)) Directory.CreateDirectory(_storagePath);
		}

		/// <summary>
		/// Ensure queue exists on disk and return a locked instance
		/// </summary>
		public IPersistentQueue PrepareQueue()
		{
			return PersistentQueue.WaitFor(_storagePath, TimeSpan.FromSeconds(10));
		}

		/// <summary>
		/// Do any cleanup after queue is disposed.
		/// </summary>
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
				Console.WriteLine("Deleting queues failed");
			}
		}
	}

	/// <summary>
	/// Factory to create a persistent queue in a default location
	/// </summary>
	public class PersistentQueueFactory : IPersistentQueueFactory
	{
		const string storagePath = "../../QUEUE"; // hacky, hacky, hacky :-(

		/// <summary>
		/// Ensure queue exists on disk and return a locked instance
		/// </summary>
		public IPersistentQueue PrepareQueue()
		{
			if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
			return PersistentQueue.WaitFor(storagePath, TimeSpan.FromSeconds(10));
		}

		/// <summary>
		/// Do any cleanup after queue is disposed.
		/// </summary>
		public void Cleanup()
		{
			// Nothing.
		}
	}
}