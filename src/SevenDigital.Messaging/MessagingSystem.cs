﻿using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.MessageSending.Loopback;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Messaging configuration and control.
	/// You should call at least one basic 'Configure' option to get a valid messaging system.
	/// </summary>
	public static class MessagingSystem
	{
		/// <summary>
		/// Basic configuration. You should call at least one basic configuration option to get a valid messaging system.
		/// </summary>
		public static readonly IMessagingConfigure Configure = new SDM_Configure();

		/// <summary>
		/// Options for adding and removing event hooks
		/// </summary>
		public static readonly IMessagingEventOptions Events = new SDM_Events();

		/// <summary>
		/// Options for inspecting test data
		/// </summary>
		public static readonly IMessagingLoopbackInformation Testing = new SDM_Testing();

		/// <summary>
		/// Runtime controls for the messaging system
		/// </summary>
		public static readonly IMessagingControl Control = new SDM_Control();
		
		/// <summary>
		/// Return a factory for setting up message handlers.
		/// You must configure messaging before calling.
		/// </summary>
		public static IReceiver Receiver()
		{
			if (!UsingLoopbackMode() && !IsConfigured())
				throw new InvalidOperationException("Receiver can't be provided: Messaging has not been configured. Try `Messaging.Configure.WithDefaults()`");

			return ObjectFactory.GetInstance<IReceiver>();
		}

		/// <summary>
		/// Get a sender node for broadcasting messages into the messaging system.
		/// You must configure messaging before calling.
		/// </summary>
		public static ISenderNode Sender()
		{
			if (!UsingLoopbackMode() && !IsConfigured())
				throw new InvalidOperationException("Sender can't be provided: Messaging has not been configured. Try `Messaging.Configure.WithDefaults()`");

			return ObjectFactory.GetInstance<ISenderNode>();
		}


		/// <summary>
		/// Returns true if in loopback mode
		/// </summary>
		internal static bool UsingLoopbackMode()
		{
			return ObjectFactory.GetAllInstances<IReceiver>().Any(n => n is LoopbackReceiver);
		}
		
		/// <summary>
		/// Returns true if messaging has been configured and not shutdown
		/// </summary>
		internal static bool IsConfigured()
		{
			//return ObjectFactory.GetAllInstances<IDispatchController>().Any(n => n is DispatchController);
			return false;
		}
	}

	#region Config interfaces
	/// <summary>
	/// Control methods for a running messaging system
	/// </summary>
	public interface IMessagingControl
	{
		/// <summary>
		/// Stop the messaging system, finishing all handlers and stopping all worker threads.
		/// After this method is called, the messaging system must be reconfigured from scratch.
		/// </summary>
		void Shutdown();
		
		/// <summary>
		/// Set maximum concurrent handlers. Set to 1 for single-thread mode
		/// </summary>
		void SetConcurrentHandlers(int max);

		/// <summary>
		/// Sets maximum number of handlers to zero. No new messages will be picked up.
		/// Use `SetConcurentHanders()` to resume.
		/// </summary>
		void Pause();
	}

	/// <summary>
	/// Messaging system loopback mode information
	/// </summary>
	public interface IMessagingLoopbackInformation
	{
		/// <summary>
		/// Return a set of all events (send, received, errors) since loopback mode was configured
		/// </summary>
		ITestEventHook LoopbackEvents();

		/// <summary>
		/// Return a list of registered listeners for a message type. Only usable in loopback mode.
		/// </summary>
		IList<Type> LoopbackListenersForMessage<T>();
	}

	/// <summary>
	/// Event hook options
	/// </summary>
	public interface IMessagingEventOptions
	{
		/// <summary>
		/// Remove all event hooks
		/// </summary>
		IMessagingEventOptions ClearEventHooks();

		/// <summary>
		/// Add an event hook.
		/// </summary>
		IMessagingEventOptions AddEventHook<T>() where T : IEventHook;
	}

	/// <summary>
	/// Initial configuration options for messaging
	/// </summary>
	public interface IMessagingConfigure
	{
		/// <summary>
		/// Configure messaging for normal use. Sets defaults for running on the local host.
		/// The returned configuration options object can be used to setup further.
		/// Calling `WithDefaults()` more than once has no effect.
		/// </summary>
		IMessagingConfigureOptions WithDefaults();

		/// <summary>
		/// Configure messaging to run in-process. This mode is for testing.
		/// All sent messages will be handled immediately, blocking the calling thread until finished.
		/// This should be called BEFORE 'WithDefaults()'
		/// Calling `WithLoopbackMode()` more than once has no effect.
		/// </summary>
		void WithLoopbackMode();
	}

	/// <summary>
	/// Optional configuration for messaging
	/// </summary>
	public interface IMessagingConfigureOptions
	{
		/// <summary>
		/// Configure RabbitMq specific management API.
		/// This makes the IRabbitMqQuery type available to query the health
		/// of your message broker cluster.
		/// </summary>
		/// <param name="host">RMQ host name or IP address</param>
		/// <param name="username">Management user name</param>
		/// <param name="password">Management password</param>
		/// <param name="vhost">Virtual host (used where appropriate)</param>
		IMessagingConfigureOptions SetManagementServer(string host, string username, string password, string vhost);
		
		/// <summary>
		/// Configure target messaging host. This should be the IP or hostname of a server 
		/// running RabbitMQ service. If you are using a non default virtual host, your host
		/// string should look like &quot;mqserver/vhost&quot;
		/// </summary>
		/// <param name="host">IP or hostname of a server running RabbitMQ service</param>
		IMessagingConfigureOptions SetMessagingServer(string host);
		
		/// <summary>
		/// Sets integration test mode. Must be set before any handlers are configured.
		/// All nodes will purge their queue on creation. All queues and exchanges with ".Integration."
		/// in their names will be deleted on disposal.
		/// </summary>
		void SetIntegrationTestMode();
	}
	#endregion

	class SDM_Control : IMessagingControl
	{
		public void Shutdown()
		{
			/*var controller = ObjectFactory.TryGetInstance<IDispatchController>();
			if (controller != null)
			{
				controller.Shutdown();
				ObjectFactory.EjectAllInstancesOf<IDispatchController>();
			}*/

			var connection = ObjectFactory.TryGetInstance<IChannelAction>();
			if (connection != null)
			{
				connection.Dispose();
				ObjectFactory.EjectAllInstancesOf<IChannelAction>();
			}
		}
		
		public void SetConcurrentHandlers(int max)
		{
			if (max < 1) throw new ArgumentException("Concurrent handlers must be at least 1", "max");
			//DestinationPoller.TaskLimit = max;
		}

		public void Pause()
		{
			//DestinationPoller.TaskLimit = 0;
		}
	}

	class SDM_Testing : IMessagingLoopbackInformation
	{
		public ITestEventHook LoopbackEvents()
		{
			if (!MessagingSystem.UsingLoopbackMode())
				throw new InvalidOperationException("Loopback events are not available: Loopback mode has not be set. Try `Messaging.Configure.WithLoopbackMode()` before your service starts.");

			return ObjectFactory.GetInstance<ITestEventHook>();
		}

		public IList<Type> LoopbackListenersForMessage<T>()
		{
			var lb = ObjectFactory.GetInstance<IReceiver>() as LoopbackReceiver;
			if (lb == null) throw new Exception("Loopback lister list is not available: Loopback mode has not be set. Try `Messaging.Configure.WithLoopbackMode()` before your service starts.");
			return lb.ListenersFor<T>();
		}
	}

	class SDM_Configure : IMessagingConfigure
	{
		public IMessagingConfigureOptions WithDefaults()
		{
			if (MessagingSystem.IsConfigured() || MessagingSystem.UsingLoopbackMode())
				return new SDM_ConfigureOptions();

			new MessagingBaseConfiguration().WithDefaults();
			Cooldown.Activate();

			ObjectFactory.Configure(map =>
			{
				map.For<IMessagingHost>().Use(() => new Host("localhost"));
				map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection("localhost"));
				map.For<IUniqueEndpointGenerator>().Use<UniqueEndpointGenerator>();
				//map.For<IDestinationPoller>().Use<DestinationPoller>();
				map.For<IMessageHandler>().Use<MessageHandler>();

				//map.For<IWorkWrapper>().Use<WorkWrapper>();
				map.For<ISleepWrapper>().Use<SleepWrapper>();
				//map.For<INode>().Use<Node>();

				//map.For<IDispatchController>().Singleton().Use<DispatchController>();
				map.For<IReceiver>().Singleton().Use<Receiver>();
				map.For<ISenderNode>().Singleton().Use<SenderNode>();
			});

			return new SDM_ConfigureOptions();
		}

		public void WithLoopbackMode()
		{
			if (MessagingSystem.UsingLoopbackMode()) return;
			if (MessagingSystem.IsConfigured())
				throw new InvalidOperationException("Messaging system has already been configured. You should set up loopback mode first.");

			new MessagingBaseConfiguration().WithDefaults();
			ObjectFactory.EjectAllInstancesOf<IReceiver>();
			//ObjectFactory.EjectAllInstancesOf<INode>();

			var factory = new LoopbackReceiver();
			ObjectFactory.Configure(map =>
			{
				map.For<IReceiver>().Singleton().Use(factory);
				map.For<ISenderNode>().Singleton().Use<LoopbackSender>().Ctor<LoopbackReceiver>().Is(factory);
				map.For<ITestEventHook>().Singleton().Use<TestEventHook>();
				//map.For<IDispatchController>().Singleton().Use<LoopbackDispatchController>();
			});


			ObjectFactory.Configure(map =>
				map.For<IEventHook>().Use(ObjectFactory.GetInstance<ITestEventHook>()));
		}
	}

	class SDM_ConfigureOptions : IMessagingConfigureOptions
	{
		public IMessagingConfigureOptions SetManagementServer(string host, string username, string password, string vhost)
		{
			new MessagingBaseConfiguration().WithRabbitManagement(host, username, password, vhost);
			return this;
		}

		public IMessagingConfigureOptions SetMessagingServer(string host)
		{
			var parts = host.Split('/');
			var hostName = parts[0];
			var virtualHost = (parts.Length > 1) ? (parts[1]) : ("/");

			ObjectFactory.Configure(map =>
			{
				map.For<IMessagingHost>().Use(() => new Host(hostName));
				map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection(hostName, virtualHost));
			});
			return this;
		}

		public void SetIntegrationTestMode()
		{
			//ObjectFactory.EjectAllInstancesOf<INode>();
			//ObjectFactory.Configure(map => map.For<INode>().Use<IntegrationTestNode>());
		}
	}

	class SDM_Events : IMessagingEventOptions
	{
		public IMessagingEventOptions ClearEventHooks()
		{
			var loopback = MessagingSystem.UsingLoopbackMode();
			ObjectFactory.EjectAllInstancesOf<IEventHook>();

			if (loopback)
			{
				ObjectFactory.Configure(map =>
					map.For<IEventHook>().Use(ObjectFactory.GetInstance<ITestEventHook>()));
			}
			return this;
		}

		public IMessagingEventOptions AddEventHook<T>() where T : IEventHook
		{
			ObjectFactory.Configure(map => map.For<IEventHook>().Add<T>());
			return this;
		}
	}
}