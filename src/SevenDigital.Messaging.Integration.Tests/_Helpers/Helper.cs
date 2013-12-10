using System;
using System.Configuration;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests._Helpers
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(server).SetIntegrationTestMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}
		
		public static void SetupTestMessagingWithoutPurging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(server);
			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}

		public static void DeleteQueue(string queueName)
		{
			try
			{
				ObjectFactory.GetInstance<IRabbitMqConnection>().WithChannel(channel => channel.QueueDelete(queueName));
			}
			catch
			{
				Ignore();
			}
		}

		static void Ignore(){}

		public static void DeleteQueueAndExchange(string queue, Type exchangeType)
		{
			try
			{
				ObjectFactory.GetInstance<IRabbitMqConnection>().WithChannel(channel => {
					channel.QueueDelete(queue);
					channel.ExchangeDelete(exchangeType.FullName);
				});
			}
			catch
			{
				Ignore();
			}
		}

		public static void SetupTestMessagingNonPersistent()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			MessagingSystem.Configure.WithDefaults().NoPersistentMessages()
				.SetMessagingServer(server).SetIntegrationTestMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}
	}

	public class TestEndpointGenerator : IUniqueEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Messaging.Integration.TestEndpoint");
		}

		public bool UseIntegrationTestName { get; set; }
	}
}