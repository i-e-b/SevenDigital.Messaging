using System;
using System.Configuration;

namespace SevenDigital.Messaging.Integration.Tests
{
	public class Helper
	{
		public static void SetupTestMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			Console.WriteLine("Using "+server+" as MQ server");
			new MessagingConfiguration().WithDefaults().WithMessagingServer(server).PurgeAllMessages();
		}
	}
}