using System;
using System.IO;
using System.Text;
using System.Threading;
using DiskQueue;
using DispatchSharp;
using DispatchSharp.QueueTypes;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.Loopback;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;
using StructureMap.Pipeline;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_Configure : IMessagingConfigure
	{
		public IMessagingConfigureOptions WithDefaults()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.IsConfigured() || MessagingSystem.UsingLoopbackMode())
					return new SDM_ConfigureOptions();

				SDM_Control.EjectAndDispose<IReceiverControl>();
				SDM_Control.EjectAndDispose<IReceiver>();

				new MessagingBaseConfiguration().WithDefaults();
				Cooldown.Activate();
				AutoShutdown.Activate();

				ObjectFactory.Configure(map => {
					// Base messaging
					map.For<IMessagingHost>().Use(() => new Host("localhost"));
					map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection("localhost"));
					map.For<IUniqueEndpointGenerator>().Singleton().Use<UniqueEndpointGenerator>();

					// threading and dispatch
					map.For<IHandlerManager>().Use<HandlerManager>();
					map.For<IPollingNodeFactory>().Use<RabbitMqPollingNodeFactory>();
					map.For<IDispatcherFactory>().Use<DispatcherFactory>();

					// singletons
					map.For<ISleepWrapper>().Singleton().Use<SleepWrapper>();
					map.For<IReceiver>().Singleton().Use<Receiver>();
					map.For<ISenderNode>().Singleton().Use<SenderNode>();
					map.For<IOutgoingQueueFactory>().Singleton().Use<PersistentQueueFactory>();

					// aliases
					map.For<IReceiverControl>().Use(() => ObjectFactory.GetInstance<IReceiver>() as IReceiverControl);
				});

				return new SDM_ConfigureOptions();
			}
		}

		public void WithLoopbackMode()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.UsingLoopbackMode()) return;
				if (MessagingSystem.IsConfigured())
					throw new InvalidOperationException("Messaging system has already been configured. You should set up loopback mode first.");

				new MessagingBaseConfiguration().WithDefaults();
				ObjectFactory.EjectAllInstancesOf<IReceiver>();

				ObjectFactory.Configure(map => {
					map.For<ILoopbackBinding>().Singleton().Use<LoopbackBinding>();
					map.For<IReceiver>().Singleton().Use<LoopbackReceiver>();
					map.For<ISenderNode>().Singleton().Use<LoopbackSender>();
					map.For<ITestEvents>().Singleton().Use<TestEvents>();
				});

				MessagingSystem.Events.AddEventHook<TestEventHook>();
			}
		}

		public void WithLocalQueue(string storagePath)
		{
			// TODO: implement!
			/*
			 * The Plan:
			 * =========
			 * 
			 * Inside `storagePath` we keep TWO DiskQueues: one for 'incoming'
			 * and one for 'dispatch'. ISenderNode does a PersistentQueue.WaitFor,
			 * adds the item, flushes and exits as fast as possible.
			 * Another thread loops, trying to get a lock on both the dispatch and
			 * incoming queue (dispatch first). When it gets a lock, it dequeues from
			 * incoming and enqueues onto dispatch.
			 * 
			 * The receiver node loops connecting to dispatch. It disconnects and sleeps
			 * if no messages. Otherwise it holds the session open, runs handlers and
			 * follows the RetryException semantics: if complete or non-retry exception
			 * the dequeue is flushed. If retry exception the dequeue is abandoned.
			 * In either case, receiver loop reads again from dispatch.
			 * 
			 * In the case of handler exceptions... I dunno. Maybe require another storage
			 * path just for handler errors.
			 */

			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.IsConfigured() || MessagingSystem.UsingLoopbackMode())
					return;// new SDM_LocalQueueOptions();

				new MessagingBaseConfiguration().WithDefaults();
				ObjectFactory.EjectAllInstancesOf<IReceiver>();

				// Base messaging
				new MessagingBaseConfiguration().WithDefaults();
				Cooldown.Activate();
				AutoShutdown.Activate();

				ObjectFactory.Configure(map => {
					// threading and dispatch
					map.For<IHandlerManager>().Use<HandlerManager>();
					map.For<IDispatcherFactory>().Use<DispatcherFactory>();

					// no-op proxy for routing
					map.For<IMessageRouter>().Use<DummyMessageRouter>();

					// singletons
					map.For<ISleepWrapper>().Singleton().Use<SleepWrapper>();
					map.For<IReceiver>().Singleton().Use<Receiver>();

					// aliases
					map.For<IReceiverControl>().Use(() => ObjectFactory.GetInstance<IReceiver>() as IReceiverControl);

					// Local queue specific
					map.For<LocalQueueConfig>().Use(new LocalQueueConfig {
						DispatchPath = Path.Combine(storagePath, "dispatch"),
						IncomingPath = Path.Combine(storagePath, "incoming")
					}
					);
					map.For<IPollingNodeFactory>().Use<LocalQueuePollingNodeFactory>();
					map.For<ISenderNode>().Singleton().Use<LocalQueueSender>();
				});
			}
		}
	}

	public class DummyMessageRouter:IMessageRouter
	{
		public void AddSource(string name) { }
		public void AddBroadcastSource(string className) { }
		public void AddDestination(string name) { }
		public void Link(string sourceName, string destinationName) { } 
		public void RouteSources(string child, string parent) { } 
		public void Send(string sourceName, string data) { } 
		public string Get(string destinationName, out ulong deliveryTag) { deliveryTag = 0; return null; } 
		public void Finish(ulong deliveryTag) { } 
		public string GetAndFinish(string destinationName) { return null; }
		public void Purge(string destinationName) { } 
		public void Cancel(ulong deliveryTag) { } 
		public void RemoveRouting(Func<string, bool> filter) { }
	}

	public class LocalQueuePollingNodeFactory:IPollingNodeFactory
	{
		readonly string _dispatchPath;
		readonly IMessageSerialiser _serialiser;
		readonly ISleepWrapper _sleeper;

		public LocalQueuePollingNodeFactory(LocalQueueConfig config,
			IMessageSerialiser serialiser, ISleepWrapper sleeper)
		{
			_serialiser = serialiser;
			_sleeper = sleeper;
			_dispatchPath = config.DispatchPath;
		}

		public ITypedPollingNode Create(IRoutingEndpoint endpoint)
		{
			return new LocalQueuePollingNode(_dispatchPath, _serialiser, _sleeper);
		}
	}

	public class LocalQueuePollingNode : ITypedPollingNode
	{
		readonly string _dispatchPath;
		readonly IMessageSerialiser _serialiser;
		readonly ISleepWrapper _sleeper;
		readonly ConcurrentSet<Type> _boundMessageTypes;

		public LocalQueuePollingNode(string dispatchPath,
			IMessageSerialiser serialiser, ISleepWrapper sleeper)
		{
			_dispatchPath = dispatchPath;
			_serialiser = serialiser;
			_sleeper = sleeper;
			_boundMessageTypes = new ConcurrentSet<Type>();
		}

		public void Enqueue(IPendingMessage<object> work)
		{
			throw new InvalidOperationException("This queue self populates and doesn't currently support direct injection.");
		}

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
				if (data == null)
				{
					session.Dispose();
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
					new PendingMessage<object>(null, msg, 0UL),
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

		public int Length()
		{
			return 0;
		}

		public bool BlockUntilReady()
		{
			return true;
		}

		public void AddMessageType(Type type)
		{
			lock (_boundMessageTypes)
			{
				_boundMessageTypes.Add(type);
			}
		}

		public void Stop()
		{
			lock (_boundMessageTypes)
			{
				_boundMessageTypes.Clear();
			}
		}
	}

	public class LocalQueueConfig
	{
		public string IncomingPath { get; set; }
		public string DispatchPath { get; set; }
	}

	public class LocalQueueSender : ISenderNode
	{
		readonly IMessageSerialiser _serialiser;
		readonly string _incomingPath;

		public LocalQueueSender(LocalQueueConfig config,
			IMessageSerialiser serialiser)
		{
			_serialiser = serialiser;
			_incomingPath = config.IncomingPath;
		}

		public void Dispose() { }

		public void SendMessage<T>(T message) where T : class, IMessage
		{
			Thread.Sleep(150);
			var data = Encoding.UTF8.GetBytes(_serialiser.Serialise(message));

			using (var queue = PersistentQueue.WaitFor(_incomingPath, TimeSpan.FromMinutes(1)))
			using (var session = queue.OpenSession())
			{
				session.Enqueue(data);
				session.Flush();
			}
			HookHelper.TrySentHooks(message);
		}
	}

}