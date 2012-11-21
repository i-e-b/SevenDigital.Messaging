using System.Configuration;
using SevenDigital.Messaging.Management;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			new MessagingConfiguration().WithDefaults().WithMessagingServer(server).PurgeAllMessages();
		}

		public static RabbitMqApi GetManagementApi()
		{
			var parts= ConfigurationManager.AppSettings["rabbitServer"].Split('/');
			
			return new RabbitMqApi("http://"+parts[0]+":55672", "guest", "guest", parts[1]);
		}
	}
}