using System;
using System.IO;
using System.Net;
using RabbitMQ.Client;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Management
{
	public class Api
	{
		private readonly Uri _managementApiHost;
		private readonly NetworkCredential _credentials;

		public Api(Uri managementApiHost, NetworkCredential credentials)
		{
			_managementApiHost = managementApiHost;
			_credentials = credentials;
		}

		public Api(string hostUri, string username, string password)
			: this (new Uri(hostUri), new NetworkCredential(username, password)){}

		public RMQueue[] ListQueues()
		{
			return JsonSerializer.DeserializeFromStream<RMQueue[]>(Get("/api/queues"));
		}

		public RMNode[] ListNodes()
		{
			return JsonSerializer.DeserializeFromStream<RMNode[]>(Get("/api/nodes"));
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
			var factory = new ConnectionFactory {
				Protocol = Protocols.FromEnvironment(), 
				HostName = _managementApiHost.Host};

			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueuePurge(queue.name);
			ch.Close();
			conn.Close();
		}
	}
}