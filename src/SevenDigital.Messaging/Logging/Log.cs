using System;
using SevenDigital.Messaging.Base;
using StructureMap;

namespace SevenDigital.Messaging.Logging
{
	/// <summary>
	/// Messaging based logger
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Log a message to the warning queue
		/// </summary>
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