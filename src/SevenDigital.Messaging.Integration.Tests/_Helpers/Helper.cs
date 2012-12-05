using System.Configuration;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;
using SevenDigital.Messaging.Base.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			new MessagingConfiguration().WithDefaults().WithMessagingServer(server);
		}

		public static IRabbitMqQuery GetManagementApi()
		{
			var parts= ConfigurationManager.AppSettings["rabbitServer"].Split('/');
			
			return new RabbitMqQuery("http://"+parts[0]+":55672", "guest", "guest", parts[1]);
		}

		public static void RemoveAllRoutingFromThisSession()
		{
			((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting();
		}

		public static void DeleteQueue(string queueName)
		{
			ObjectFactory.GetInstance<IRabbitMqConnection>().WithChannel(channel=>channel.QueueDelete(queueName));
		}
	}
}