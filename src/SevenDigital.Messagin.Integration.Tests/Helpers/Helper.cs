using System.Configuration;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			new MessagingConfiguration().WithDefaults().WithMessagingServer(
				ConfigurationManager.AppSettings["rabbitServer"]
				).PurgeAllMessages();
		}
	}
}