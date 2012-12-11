using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.MessageSending.Loopback;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Configuration helper for structure map and SevenDigital.Messaging
	/// </summary>
	public class MessagingConfiguration
	{
		/// <summary>
		/// Configure SevenDigital.Messaging with defaults.
		/// After calling this method, you can use the INodeFactory as a collaborator.
		/// The default host is "localhost"
		/// </summary>
		public MessagingConfiguration WithDefaults()
		{
			if (UsingLoopbackMode()) return this;

			new  MessagingBaseConfiguration().WithDefaults();
			Cooldown.Activate();

			ObjectFactory.Configure(map => {
				map.For<IMessagingHost>().Use(()=> new Host("localhost"));
				map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection("localhost"));
				map.For<ISenderEndpointGenerator>().Use<SenderEndpointGenerator>();
				map.For<IUniqueEndpointGenerator>().Use<UniqueEndpointGenerator>();
				map.For<IDestinationPoller>().Use<DestinationPoller>();
				map.For<IMessageDispatcher>().Use<MessageDispatcher>();


				map.For<IThreadPoolWrapper>().Use<ThreadPoolWrapper>();
				map.For<ISleepWrapper>().Use<SleepWrapper>();
				map.For<INode>().Use<Node>();

				
				map.For<IDispatchController>().Singleton().Use<DispatchController>();
				map.For<INodeFactory>().Singleton().Use<NodeFactory>();
                map.For<ISenderNode>().Singleton().Use<SenderNode>();
			});

			return this;
		}

		static bool UsingLoopbackMode()
		{
			return ObjectFactory.GetAllInstances<INodeFactory>().Any(n=>n is LoopbackNodeFactory);
		}

		/// <summary>
		/// Configure target messaging host. This should be the IP or hostname of a server 
		/// running RabbitMQ service.
		/// </summary>
		/// <param name="hostPath">IP or hostname of a server running RabbitMQ service</param>
		public MessagingConfiguration WithMessagingServer(string hostPath)
		{
			var parts = hostPath.Split('/');
			var host = parts[0];
			var vhost = (parts.Length > 1) ? (parts[1]) : ("/");

			ObjectFactory.Configure(map =>
			{
				map.For<IMessagingHost>().Use(() => new Host(host));
				map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection(host, vhost));
			});
			return this;
		}

		/// <summary>
		/// Add an event hook the the messaging system
		/// </summary>
		public MessagingConfiguration AddEventHook<T>() where T : IEventHook
		{
			ObjectFactory.Configure(map => map.For<IEventHook>().Add<T>());
			return this;
		}

		/// <summary>
		/// Remove all event hooks from the event system
		/// </summary>
		public MessagingConfiguration ClearEventHooks()
		{
			var loopback = UsingLoopbackMode();
			ObjectFactory.EjectAllInstancesOf<IEventHook>();

			if (loopback)
			{
				ObjectFactory.Configure(map =>
					map.For<IEventHook>().Use(ObjectFactory.GetInstance<ITestEventHook>()));
			}
			return this;
		}
		
		/// <summary>
		/// Configure SevenDigital.Messaging with loopback communications for testing.
		/// After calling this method, you can use the INodeFactory as a collaborator.
		/// Messages will trigger instantly. DO NOT use for production!
		/// </summary>
		public MessagingConfiguration WithLoopback()
		{
			new  MessagingBaseConfiguration().WithDefaults();
			ObjectFactory.EjectAllInstancesOf<INodeFactory>();
			ObjectFactory.EjectAllInstancesOf<INode>();

		    var factory = new LoopbackNodeFactory();
			ObjectFactory.Configure(map =>
			{
                map.For<INodeFactory>().Singleton().Use(factory);
                map.For<ISenderNode>().Singleton().Use<LoopbackSender>().Ctor<LoopbackNodeFactory>().Is(factory);
				map.For<ITestEventHook>().Singleton().Use<TestEventHook>();
				map.For<IDispatchController>().Singleton().Use<LoopbackDispatchController>();
	        });


			ObjectFactory.Configure(map =>
				map.For<IEventHook>().Use(ObjectFactory.GetInstance<ITestEventHook>()));

			return this;
		}

		/// <summary>
		/// Return a hook which captures all events in a loopback session
		/// </summary>
		public ITestEventHook LoopbackEvents()
		{
			return ObjectFactory.GetInstance<ITestEventHook>();
		}

		/// <summary>
		/// Return registered listeners for a message type. Only usable in loopback mode.
		/// </summary>
		public List<Type> LoopbackListenersForMessage<T>()
		{
			var lb = ObjectFactory.GetInstance<INodeFactory>() as LoopbackNodeFactory;
			if (lb == null) throw new Exception("Not in loopback mode");
			return lb.ListenersFor<T>();
		}


		/// <summary>
		/// Sets integration test mode. This mode cannot be in a running process.
		/// All nodes will purge their queue on creation. All queues and exchanges with ".Integration."
		/// in their names will be deleted on disposal.
		/// </summary>
		public MessagingConfiguration IntegrationTestMode()
		{
			ObjectFactory.EjectAllInstancesOf<INode>();
			ObjectFactory.Configure(map => map.For<INode>().Use<IntegrationTestNode>());
			return this;
		}

		/// <summary>
		/// Stop the messaging system.
		/// </summary>
		public void Shutdown()
		{
			var controller = ObjectFactory.TryGetInstance<IDispatchController>();
			if (controller != null) controller.Shutdown();
		}
	}
}
