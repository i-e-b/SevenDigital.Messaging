using System;
using SevenDigital.Messaging.Base;
using StructureMap;

namespace SevenDigital.Messaging.Logging
{
	public class Log
	{
		public static void Warning(string message)
		{
			try
			{
				var mb = ObjectFactory.GetInstance<IMessagingBase>();
				mb.CreateDestination<ILogMessage>("WarningLog");
				mb.SendMessage(new LogMessage{Message = message});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Messaging is broken? " + ex.GetType().Name + ": " + ex.Message);
			}
		}
	}
}