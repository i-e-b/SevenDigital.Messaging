using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.ConfigurationActions;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.Loopback;
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
		public static readonly IMessagingTestingMethods Testing = new SDM_Testing();

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
				ObjectFactory.GetAllInstances<IReceiver>().Any(n => n is LoopbackReceiver);
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

		internal static TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(10);
		internal static readonly object ConfigurationLock = new object();
		internal static int Concurrency = DispatchSharp.Internal.Default.ThreadCount;
	}

	/// <summary>
	/// Control methods for a running messaging system
	/// </summary>
	public interface IMessagingControl
	{
		/// <summary>
		/// Stop the messaging system, finishing all handlers and stopping all worker threads.
		/// After this method is called, the messaging system must be reconfigured from scratch.
		/// This will wait up to 10 seconds for sending messages to complete. If you want to increase the 
		/// time-out, use `MessagingSystem.Control.SetShutdownTimeout(...)`
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Set the maximum time to wait for pending messages to send during shutdown
		/// </summary>
		void SetShutdownTimeout(TimeSpan maxWait);

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
	public interface IMessagingTestingMethods
	{
		/// <summary>
		/// Return a set of all events (send, received, errors) since loopback mode was configured
		/// </summary>
		ITestEvents LoopbackEvents();

		/// <summary>
		/// Return a list of registered listeners for a message type. Only usable in loopback mode.
		/// </summary>
		[Obsolete("Use `LoopbackHandlers().ForMessage<T>()` instead")]
		IEnumerable<Type> LoopbackListenersForMessage<T>();

		/// <summary>
		/// Handler bindings that have been registered during loopback mode sessions.
		/// </summary>
		ILoopbackBinding LoopbackHandlers();

		/// <summary>
		/// Add test event hooks when not in loopback mode.
		/// </summary>
		void AddTestEventHook();

		/// <summary>
		/// returns the current set concurrency limit --
		/// this is the maximum number of handlers that will run at once
		/// </summary>
		int ConcurrencyLimit();
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
		
		/// <summary>
		/// Use a local-machine persistent queue for process-to-process message sending
		/// that does not rely on both sides being connected at once.
		/// <para>This is not high performance for multiple-readers or high write loads</para>
		/// </summary>
		/// <param name="storagePath">File path for storing queue items. This should either not
		/// be created or have been created previously by `SetLocalQueue`</param>
		void WithLocalQueue(string storagePath);
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
		/// By default, the messaging system will try to do store-and-forward
		/// messaging. This is persisted to permanent storage.
		/// <para>This option turns store-and-forward off. No disk files will
		/// be required, but in the event of total failure, messages will be lost.</para>
		/// </summary>
		IMessagingConfigureOptions NoPersistentMessages();

		/// <summary>
		/// Sets integration test mode. Must be set before any handlers are configured.
		/// All nodes will purge their queue on creation. All queues and exchanges with ".Integration."
		/// in their names will be deleted on disposal.
		/// </summary>
		void SetIntegrationTestMode();
	}
}