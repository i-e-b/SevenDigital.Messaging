using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.EventHooks;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Configuration helper for structure map and SevenDigital.Messaging
	/// </summary>
	[Obsolete("MessagingConfiguration is deprecated. Please use the 'MessagingSystem' static class", false)]
	public class MessagingConfiguration
	{
		/// <summary>
		/// Configure SevenDigital.Messaging with defaults.
		/// After calling this method, you can use the INodeFactory as a collaborator.
		/// The default host is "localhost"
		/// </summary>
		public MessagingConfiguration WithDefaults()
		{
			MessagingSystem.Configure.WithDefaults();

			return this;
		}

		/// <summary>
		/// Configure RabbitMq specific management API.
		/// This makes the IRabbitMqQuery type available to query the health
		/// of your message broker cluster.
		/// </summary>
		/// <param name="host">RMQ host name or IP address</param>
		/// <param name="username">Management user name</param>
		/// <param name="password">Management password</param>
		/// <param name="vhost">Virtual host (used where appropriate)</param>
		public MessagingConfiguration WithMessagingQuery(string host, string username, string password, string vhost)
		{
			MessagingSystem.Configure.WithDefaults().SetManagementServer(host, username, password, vhost);
			return this;
		}

		/// <summary>
		/// Returns true if in loopback mode
		/// </summary>
		public bool UsingLoopbackMode()
		{
			return MessagingSystem.UsingLoopbackMode();
		}

		/// <summary>
		/// Configure target messaging host. This should be the IP or hostname of a server 
		/// running RabbitMQ service.
		/// </summary>
		/// <param name="hostPath">IP or hostname of a server running RabbitMQ service</param>
		public MessagingConfiguration WithMessagingServer(string hostPath)
		{
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(hostPath);
			return this;
		}

		/// <summary>
		/// Add an event hook the the messaging system
		/// </summary>
		public MessagingConfiguration AddEventHook<T>() where T : IEventHook
		{
			MessagingSystem.Events.AddEventHook<T>();
			return this;
		}

		/// <summary>
		/// Remove all event hooks from the event system
		/// </summary>
		public MessagingConfiguration ClearEventHooks()
		{
			MessagingSystem.Events.ClearEventHooks();
			return this;
		}

		/// <summary>
		/// Configure SevenDigital.Messaging with loopback communications for testing.
		/// After calling this method, you can use the INodeFactory as a collaborator.
		/// Messages will trigger instantly. DO NOT use for production!
		/// </summary>
		public MessagingConfiguration WithLoopback()
		{
			MessagingSystem.Configure.WithLoopbackMode();
			MessagingSystem.Testing.LoopbackEvents().Reset();
			return this;
		}

		/// <summary>
		/// Return a hook which captures all events in a loopback session
		/// </summary>
		public ITestEvents LoopbackEvents()
		{
			return MessagingSystem.Testing.LoopbackEvents();
		}

		/// <summary>
		/// Return registered listeners for a message type. Only usable in loopback mode.
		/// </summary>
		public IList<Type> LoopbackListenersForMessage<T>()
		{
			return MessagingSystem.Testing.LoopbackListenersForMessage<T>().ToList();
		}

		/// <summary>
		/// Sets integration test mode. This mode cannot be in a running process.
		/// All nodes will purge their queue on creation. All queues and exchanges with ".Integration."
		/// in their names will be deleted on disposal.
		/// </summary>
		public MessagingConfiguration IntegrationTestMode()
		{
			MessagingSystem.Configure.WithDefaults().SetIntegrationTestMode();
			return this;
		}

		/// <summary>
		/// Set maximum concurrent handlers. Set to 1 for single-thread mode
		/// </summary>
		public MessagingConfiguration ConcurrentHandlers(int max)
		{
			MessagingSystem.Control.SetConcurrentHandlers(max);
			return this;
		}

		/// <summary>
		/// Stop the messaging system.
		/// </summary>
		public void Shutdown()
		{
			MessagingSystem.Control.Shutdown();
		}
	}
}