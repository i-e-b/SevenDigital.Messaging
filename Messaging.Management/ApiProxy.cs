using System;
using System.IO;
using System.Net;
using System.Text;
using RabbitMQ.Client;
using ServiceStack.Text;

namespace RemoteRabbitTool
{
	public class ApiProxy
	{
		private readonly Uri _managementApiHost;
		private readonly NetworkCredential _credentials;

		public ApiProxy(Uri managementApiHost, NetworkCredential credentials)
		{
			_managementApiHost = managementApiHost;
			_credentials = credentials;
		}

		public ApiProxy(string hostUri, string username, string password)
			: this (new Uri(hostUri), new NetworkCredential(username, password)){}

		public RMQueue[] ListQueues()
		{
			return JsonSerializer.DeserializeFromString<RMQueue[]>(Get("/api/queues"));
		}

		public RMNode[] ListNodes()
		{
			return JsonSerializer.DeserializeFromString<RMNode[]>(Get("/api/nodes"));
		}

		public string Get(string endpoint)
		{
			Uri result;

			if (Uri.TryCreate(_managementApiHost, endpoint, out result))
			{

				var webRequest = WebRequest.Create(result);
				webRequest.Credentials = _credentials;

				return ReadAll(webRequest.GetResponse().GetResponseStream());
			}

			return null;
		}


		public void PurgeQueue(RMQueue queue)
		{
			var factory = new ConnectionFactory();
			factory.Protocol = Protocols.FromEnvironment();
			factory.HostName = _managementApiHost.Host;
			var conn = factory.CreateConnection();
			var ch = conn.CreateModel();
			ch.QueuePurge(queue.name);
			ch.Close();
			conn.Close();
		}

		string ReadAll(Stream stream)
		{
			var ms = new MemoryStream();
			using (stream)
			{
				int d;
				while ((d = stream.ReadByte()) >= 0)
				{
					ms.WriteByte((byte)d);
				}
			}
			return Encoding.UTF8.GetString(ms.ToArray());
		}
	}
}