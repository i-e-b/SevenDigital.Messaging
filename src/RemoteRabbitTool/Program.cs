using System;
using System.Configuration;

namespace RemoteRabbitTool
{
	class Program
	{

		static void Main()
		{
			var proxyService = new ApiProxy(ManagementUri, ApiUsername, ApiPassword);

			var nodes = proxyService.ListNodes();

			foreach (var node in nodes)
			{
				Console.WriteLine("Node '"+node.name+"' is "+ (node.AnyAlarms() ? "BROKENS!" : "Ok"));
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

		static string ApiUsername
		{
			get { return ConfigurationManager.AppSettings["ApiUsername"];}
		}
		static string MessageHost
		{
			get { return ConfigurationManager.AppSettings["MessageHost"]; }
		}
		static string ManagementUri
		{
			get { return "http://"+MessageHost+":55672"; }
		}
		static string ApiPassword
		{
			get { return ConfigurationManager.AppSettings["ApiPassword"];}
		}
	}
}
