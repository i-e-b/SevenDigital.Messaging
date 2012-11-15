using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using RabbitMQ.Client;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Management
{
	public class RabbitMqApi
	{
		readonly string virtualHost;
		readonly Uri _managementApiHost;
		readonly NetworkCredential _credentials;
		readonly string slashHost;

		/// <summary>
		/// Uses app settings: "Messaging.Host", "ApiUsername", "ApiPassword"
		/// </summary>
		public static RabbitMqApi WithConfigSettings()
		{
			var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
			var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
			var username = ConfigurationManager.AppSettings["ApiUsername"];
			var password = ConfigurationManager.AppSettings["ApiPassword"];
			var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

			return new RabbitMqApi("http://"+hostUri+":55672", username, password, vhost);
		}

		public RabbitMqApi(Uri managementApiHost, NetworkCredential credentials)
		{
			_managementApiHost = managementApiHost;
			_credentials = credentials;
		}

		public RabbitMqApi(string hostUri, string username, string password, string virtualHost = "/")
			: this(new Uri(hostUri), new NetworkCredential(username, password))
		{
			this.virtualHost = virtualHost;
			slashHost = (virtualHost.StartsWith("/")) ? (virtualHost) : ("/" + virtualHost);
		}

		public RMQueue[] ListQueues()
		{
			using (var stream = Get("/api/queues"+slashHost))
				return JsonSerializer.DeserializeFromStream<RMQueue[]>(stream);
		}

		public RMNode[] ListNodes()
		{
			using (var stream = Get("/api/nodes"))
				return JsonSerializer.DeserializeFromStream<RMNode[]>(stream);
		}

		public RMExchange[] ListExchanges()
		{
			using (var stream = Get("/api/exchanges"+slashHost))
				return JsonSerializer.DeserializeFromStream<RMExchange[]>(stream);
		}

		public Stream Get(string endpoint)
		{
			Uri result;

			if (Uri.TryCreate(_managementApiHost, endpoint, out result))
			{
				var webRequest = WebRequest.Create(result);
				webRequest.Credentials = _credentials;

				return webRequest.GetResponse().GetResponseStream();
			}

			return null;
		}

		public void PurgeQueue(RMQueue queue)
		{
			var factory = new ConnectionFactory
			{
				Protocol = Protocols.FromEnvironment(),
				HostName = _managementApiHost.Host,
				VirtualHost = virtualHost
			};

			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueuePurge(queue.name);
			ch.Close();
			conn.Close();
		}

		public void DeleteQueue(string queueName)
		{
			var factory = new ConnectionFactory
			{
				Protocol = Protocols.FromEnvironment(),
				HostName = _managementApiHost.Host,
				VirtualHost = virtualHost
			};

			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueueDelete(queueName);

			if (ListExchanges().Any(e => e.name == queueName)) ch.ExchangeDelete(queueName);
			ch.Close();
			conn.Close();
		}

		public void AddQueue(string queueName)
		{
			var factory = new ConnectionFactory
			{
				Protocol = Protocols.FromEnvironment(),
				HostName = _managementApiHost.Host,
				VirtualHost = virtualHost
			};
			
			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueueDeclare(queueName, true, false, false, new Dictionary<string,string>());
			ch.Close();
			conn.Close();
		}
	}
}