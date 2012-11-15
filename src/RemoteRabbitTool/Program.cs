using System;
using System.Configuration;
using SevenDigital.Messaging.Management;

namespace RemoteRabbitTool
{
	class Program
	{
		static void Main()
		{
			var proxyService = RabbitMqApi.WithConfigSettings();

			var nodes = proxyService.ListNodes();

			foreach (var node in nodes)
			{
				Console.WriteLine("Node '"+node.name+"' -->");
				Console.WriteLine((node.AnyAlarms() ? "    ALERT!" : "    Ok"));
				Console.WriteLine("    Memory "+node.FreeMemPercent()+"% free");
				Console.WriteLine("    Disk "+node.FreeDisk()+" free");
			}

			Console.WriteLine("-------------------------------");
			Console.WriteLine("         Queue State           ");
			Console.WriteLine("-------------------------------");
			foreach (var queue in proxyService.ListQueues())
			{
				Console.WriteLine(queue.name);
				if (queue.messages > 50)
				{
					Console.WriteLine("    it's a bit full. I'll purge it now");
					proxyService.PurgeQueue(queue);
				}
			}
			

			Console.WriteLine("[Enter] to quit");
			Console.ReadKey();
		}
	}
}
