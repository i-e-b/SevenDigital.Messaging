using System;
using System.IO;
using System.Net;
using System.Text;

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