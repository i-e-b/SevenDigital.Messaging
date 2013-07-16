using System;
using DiskQueue;

namespace SevenDigital.Messaging.MessageSending
{
	public interface IPersistentQueueFactory
	{
		IPersistentQueue PrepareQueue(string storagePath);
	}

	public class PersistentQueueFactory : IPersistentQueueFactory
	{
		public IPersistentQueue PrepareQueue(string storagePath)
		{
			return PersistentQueue.WaitFor(storagePath, TimeSpan.FromSeconds(30));
		}
	}
}