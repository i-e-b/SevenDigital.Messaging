using System;
using SevenDigital.Messaging.Base;
using StructureMap;

namespace SevenDigital.Messaging.Dispatch
{
	public class Log
	{
		public static void Warning(string message)
		{
			try
			{
				var mb = ObjectFactory.TryGetInstance<IMessagingBase>();
				if (mb == null) return;
				mb.CreateDestination<LogMessage>("WarningLog");
			}
			catch
			{
				Console.WriteLine("Messaging is broken!");
			}
		}
	}
}