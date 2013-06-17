using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.Loopback;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.MessageSending;
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
		/// Runtime controls for the messaging system, including
		/// threading limits and shutdown.
		/// </summary>
		public static readonly IMessagingControl Control = new SDM_Control();

		/// <summary>
		/// Return a factory for setting up message handlers.
		/// You must configure messaging before calling.
		/// </summary>
		public static IReceiver Receiver()
		{
			if (!UsingLoopbackMode() && !IsConfigured())
				throw new InvalidOperationException("Receiver can't be provided: Messaging has not been configured. Try `MessagingSystem.Configure.WithDefaults()`");

			return ObjectFactory.GetInstance<IReceiver>();
		}

		/// <summary>
		/// Get a sender node for broadcasting messages into the messaging system.
		/// You must configure messaging before calling.
		/// </summary>
		public static ISenderNode Sender()
		{
			if (!UsingLoopbackMode() && !IsConfigured())
				throw new InvalidOperationException("Sender can't be provided: Messaging has not been configured. Try `MessagingSystem.Configure.WithDefaults()`");

			return ObjectFactory.GetInstance<ISenderNode>();
		}

		/// <summary>
		/// Returns true if in loopback mode
		/// </summary>
		internal static bool UsingLoopbackMode()
		{
			return 
				ObjectFactory.GetAllInstances<IReceiver>().Any(n => n is LoopbackReceiver)
				&& ObjectFactory.TryGetInstance<ISenderNode>() != null;
		}

		/// <summary>
		/// Returns true if messaging has been configured and not shutdown
		/// </summary>
		internal static bool IsConfigured()
		{
			return 
				ObjectFactory.GetAllInstances<IReceiver>().Any(n => n is IReceiverControl)
				&& ObjectFactory.Model.HasImplementationsFor<ISenderNode>();
		}

		internal static readonly object ConfigurationLock = new object();
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

		/// <summary>
		/// Register an action to take on messaging warnings.
		/// </summary>
		void OnInternalWarning(Action<MessagingLogEventArgs> action);
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
		IEnumerable<Type> LoopbackListenersForMessage<T>();
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

				var factory = new LoopbackReceiver();
				ObjectFactory.Configure(map => {
					map.For<IReceiver>().Singleton().Use(factory);
					map.For<ISenderNode>().Singleton().Use<LoopbackSender>().Ctor<LoopbackReceiver>().Is(factory);
					map.For<ITestEventHook>().Singleton().Use<TestEventHook>();
				});


				ObjectFactory.Configure(map => map.For<IEventHook>().Use(ObjectFactory.GetInstance<ITestEventHook>()));
			}
		}
	}

	class SDM_Control : IMessagingControl
	{
		public void Shutdown()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				Log.Instance().Shutdown();

				if (!MessagingSystem.UsingLoopbackMode())
				{
					EjectAndDispose<IReceiverControl>();
					EjectAndDispose<IReceiver>();
				}

				EjectAndDispose<ISenderNode>();

				EjectAndDispose<IUniqueEndpointGenerator>();
				EjectAndDispose<ISleepWrapper>();
				EjectAndDispose<IEventHook>();

				EjectAndDispose<IMessagingHost>();
				EjectAndDispose<IRabbitMqConnection>();
				EjectAndDispose<IChannelAction>();
			}
		}

		public void OnInternalWarning(Action<MessagingLogEventArgs> action)
		{
			Log.Instance().RegisterAction(action);
		}

		public static void EjectAndDispose<T>()
		{
			List<IDisposable> instances;

			try
			{
				instances = ObjectFactory.GetAllInstances<T>().OfType<IDisposable>().ToList();
			}
			catch
			{
				instances = new List<IDisposable>();
			}

			ObjectFactory.EjectAllInstancesOf<T>();
			if (instances.Count < 1) return;

			foreach (var disposable in instances) disposable.Dispose();
		}

		public void SetConcurrentHandlers(int max)
		{
			if (max < 1) throw new ArgumentException("Concurrent handlers must be at least 1", "max");
			var controller = ObjectFactory.TryGetInstance<IReceiver>() as IReceiverControl;
			if (controller == null) throw new InvalidOperationException("Messaging is not configured");

			controller.SetConcurrentHandlers(max);
		}

		public void Pause()
		{
			var controller = ObjectFactory.TryGetInstance<IReceiver>() as IReceiverControl;
			if (controller == null) throw new InvalidOperationException("Messaging is not configured");
			controller.SetConcurrentHandlers(0);
		}
	}

	class SDM_Testing : IMessagingLoopbackInformation
	{
		public ITestEventHook LoopbackEvents()
		{
			if (!MessagingSystem.UsingLoopbackMode())
				throw new InvalidOperationException("Loopback events are not available: Loopback mode has not be set. Try `MessagingSystem.Configure.WithLoopbackMode()` before your service starts.");

			return ObjectFactory.GetInstance<ITestEventHook>();
		}

		public IEnumerable<Type> LoopbackListenersForMessage<T>()
		{
			var lb = ObjectFactory.GetInstance<IReceiver>() as LoopbackReceiver;
			if (lb == null) 
				throw new Exception("Loopback lister list is not available: Loopback mode has not be set. Try `MessagingSystem.Configure.WithLoopbackMode()` before your service starts.");

			return lb.ListenersFor<T>();
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
			if (MessagingSystem.UsingLoopbackMode())
				throw new Exception("Integration test mode can not be used in loopback mode");

			var controller = ObjectFactory.TryGetInstance<IReceiver>() as IReceiverControl;
			if (controller == null)
				throw new Exception("Messaging is not configured");

			var namer = ObjectFactory.TryGetInstance<IUniqueEndpointGenerator>();
			if (namer == null) throw new Exception("Unique endpoint generator was not properly configured.");

			namer.UseIntegrationTestName = true;
			controller.PurgeOnConnect = true;
			controller.DeleteIntegrationEndpointsOnShutdown = true;
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