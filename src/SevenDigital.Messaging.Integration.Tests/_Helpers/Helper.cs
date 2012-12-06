using System.Configuration;
using SevenDigital.Messaging.Base.RabbitMq;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			new MessagingConfiguration().WithDefaults().WithMessagingServer(server)
				.IntegrationTestMode();
		}
		public static void DeleteQueue(string queueName)
		{
			ObjectFactory.GetInstance<IRabbitMqConnection>().WithChannel(channel=>channel.QueueDelete(queueName));
		}
	}
}