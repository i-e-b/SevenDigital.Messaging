using System;
using System.Text;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;

namespace SevenDigital.Messaging.MessageReceiving.LocalQueue
{
	public class LocalQueuePollingNode : ITypedPollingNode
	{
		readonly string _dispatchPath;
		readonly string _incomingPath;
		readonly IMessageSerialiser _serialiser;
		readonly ISleepWrapper _sleeper;
		readonly ConcurrentSet<Type> _boundMessageTypes;

		public LocalQueuePollingNode(string dispatchPath, string incomingPath,
		                             IMessageSerialiser serialiser, ISleepWrapper sleeper)
		{
			_dispatchPath = dispatchPath;
			_incomingPath = incomingPath;
			_serialiser = serialiser;
			_sleeper = sleeper;
			_boundMessageTypes = new ConcurrentSet<Type>();
		}

		/// <summary> Not supported </summary>
		public void Enqueue(IPendingMessage<object> work)
		{
			throw new InvalidOperationException("This queue self populates and doesn't currently support direct injection.");
		}

		/// <summary>
		/// Try and get an item from this queue.
		/// Success is encoded in the <see cref="WorkQueueItem{T}"/> result 'HasItem' 
		/// </summary>
		public IWorkQueueItem<IPendingMessage<object>> TryDequeue()
		{
			if (_boundMessageTypes.Count < 1)
			{
				_sleeper.SleepMore();
				return new WorkQueueItem<IPendingMessage<object>>();
			}

			IPersistentQueue[] queue = {PersistentQueue.WaitFor(_dispatchPath, TimeSpan.FromMinutes(1))};
			if (queue[0] == null) throw new Exception("Unexpected null queue");
			try
			{
				var session = queue[0].OpenSession();
				var data = session.Dequeue();
				if (data == null) // queue is empty
				{
					session.Dispose();
					TryPumpingMessages(queue[0]);
					queue[0].Dispose();
					_sleeper.SleepMore();

					return new WorkQueueItem<IPendingMessage<object>>();
				}

				object msg;
				try
				{
					msg = _serialiser.DeserialiseByStack(Encoding.UTF8.GetString(data));
				}
				catch
				{
					session.Dispose();
					throw;
				}

				_sleeper.Reset();
				return new WorkQueueItem<IPendingMessage<object>>(
					new PendingMessage<object>(new DummyRouter(), msg, 0UL),
					m => { // Finish a message
						session.Flush();
						session.Dispose();
						if (queue[0] != null) queue[0].Dispose();
						queue[0] = null;
					},
					m => {
						// Cancel a message
						session.Dispose();
						if (queue[0] != null) queue[0].Dispose();
						queue[0] = null;
					});
			}
			catch
			{
				if (queue[0] != null) queue[0].Dispose();
				throw;
			}
		}

		/// <summary>
		/// Try to move messages from the incoming queue to the dispatch queue
		/// </summary>
		void TryPumpingMessages(IPersistentQueue dispatchQueue)
		{
			try
			{
				using (var incomingQueue = PersistentQueue.WaitFor(_incomingPath, TimeSpan.FromSeconds(1)))
				using (var dst = dispatchQueue.OpenSession())
				using (var src = incomingQueue.OpenSession())
				{
					byte[] data;
					while ((data = src.Dequeue()) != null)
					{
						dst.Enqueue(data);
						dst.Flush();
						src.Flush();
					}
				}
			}
			catch (TimeoutException)
			{
				Ignore();
			}
		}

		/// <summary> Ignore exceptions of this type </summary>
		static void Ignore() { }

		/// <summary>
		/// Approximate snapshot length.
		/// <para>Always returns zero in this instance.</para>
		/// </summary>
		public int Length() { return 0; }

		/// <summary>
		/// Advisory method: block if the queue is waiting to be populated.
		/// <para>Always immediately returns true in this instance.</para>
		/// </summary>
		public bool BlockUntilReady() { return true; }

		/// <summary>
		/// A message type for which to poll
		/// </summary>
		public void AddMessageType(Type type)
		{
			lock (_boundMessageTypes)
			{
				_boundMessageTypes.Add(type);
			}
		}

		/// <summary>
		/// Stop receiving messages
		/// <para>Deregisters all message types</para>
		/// </summary>
		public void Stop()
		{
			lock (_boundMessageTypes)
			{
				_boundMessageTypes.Clear();
			}
		}
	}
}