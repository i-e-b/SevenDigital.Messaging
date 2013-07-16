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
					map.For<IPersistentQueueFactory>().Singleton().Use<IntegrationTestQueueFactory>());

				namer.UseIntegrationTestName = true;
				controller.PurgeOnConnect = true;
				controller.DeleteIntegrationEndpointsOnShutdown = true;
			}
		}
	}
}