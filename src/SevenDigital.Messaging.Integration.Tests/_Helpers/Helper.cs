using System.Configuration;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			Messaging.Configure.WithDefaults().SetMessagingServer(server).SetIntegrationTestMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}
		
		public static void SetupTestMessagingWithoutPurging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			Messaging.Configure.WithDefaults().SetMessagingServer(server);
		}
		public static void DeleteQueue(string queueName)
		{
			try
			{
				ObjectFactory.GetInstance<IRabbitMqConnection>().WithChannel(channel => channel.QueueDelete(queueName));
			} catch
			{
				Ignore();
			}
		}

		static void Ignore(){}
	}

	public class TestEndpointGenerator : IUniqueEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Messaging.Integration.TestEndpoint");
		}
	}
}