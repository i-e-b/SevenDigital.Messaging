using System;
using System.Configuration;
using System.Net;
using RabbitMQ.Client;
using ServiceStack.Text;

namespace RemoteRabbitTool
{
	class Program
	{

		static void Main()
		{
			//Console.WriteLine("Which RabbitMQ instance?");
			//var target = Console.ReadLine();

			
			var proxyService = new ApiProxy(
				new Uri(ManagementUri), 
				new NetworkCredential(ApiUsername, ApiPassword)
			);

			var queues = JsonSerializer.DeserializeFromString<RMQueue[]>(proxyService.Get("/api/queues"));

			foreach (var queue in queues)
			{
				Console.WriteLine(queue.name);
				PurgeQueue(MessageHost, queue.name);
			}


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

		static void PurgeQueue(string host, string queue)
		{
			var factory = new ConnectionFactory();
			factory.Protocol = Protocols.FromEnvironment();
			factory.HostName = host;
			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueuePurge(queue);
			ch.Close();
			conn.Close();
		}
	}
}
