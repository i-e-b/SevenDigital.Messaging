using System;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_ConfigureOptions : IMessagingConfigureOptions
	{
		[Obsolete("Please use SetManagementServer(string host, int port, string username, string password, string vhost) instead.")]
		public IMessagingConfigureOptions SetManagementServer(string host, string username, string password, string vhost)
		{
			return SetManagementServer(host, 55672, username, password, vhost);
		}

		public IMessagingConfigureOptions SetManagementServer(string host, int port, string username, string password, string vhost)
		{
			new MessagingBaseConfiguration().WithRabbitManagement(host, port, username, password, vhost);
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

		/// <summary>
		/// By default, the messaging system will try to do store-and-forward
		/// messaging. This is persisted to permanent storage.
		/// This option turns store-and-forward off. No disk files will
		/// be required, but in the event of total failure, messages will be lost.
		/// </summary>
		public IMessagingConfigureOptions NoPersistentMessages()
		{
			ObjectFactory.EjectAllInstancesOf<IOutgoingQueueFactory>();
			
			ObjectFactory.Configure(map => map.For<IOutgoingQueueFactory>().Use<NonPersistentQueueFactory>());

			return this;
		}

		public void SetIntegrationTestMode()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.UsingLoopbackMode())
					throw new Exception("Integration test mode can not be used in loopback mode");

				var controller = ObjectFactory.TryGetInstance<IReceiver>() as IReceiverControl;
				if (controller == null)
					throw new Exception("Messaging is not configured");

				var namer = ObjectFactory.TryGetInstance<IUniqueEndpointGenerator>();
				if (namer == null) throw new Exception("Unique endpoint generator was not properly configured.");

				ObjectFactory.Configure(map =>
					map.For<IOutgoingQueueFactory>().Singleton().Use<IntegrationTestQueueFactory>());

				//NoPersistentMessages();

				namer.UseIntegrationTestName = true;
				controller.PurgeOnConnect = true;
				controller.DeleteIntegrationEndpointsOnShutdown = true;
			}
		}
	}
}