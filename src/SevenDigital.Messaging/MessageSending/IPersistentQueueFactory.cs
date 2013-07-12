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
			return new PersistentQueue(storagePath);
		}
	}
}