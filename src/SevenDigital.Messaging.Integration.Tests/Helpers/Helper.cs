using System.Configuration;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			new MessagingConfiguration().WithDefaults().WithMessagingServer(server).PurgeAllMessages();
		}
	}
}