using System.Collections.Generic;

namespace SevenDigital.Messaging.Management
{
	public class RMExchange: IRMExchange
	{
		public string name { get; set; }
		public string vhost { get; set; }
		public string type { get; set; }
		public bool durable { get; set; }
		public bool auto_delete { get; set; }
		public bool @internal { get; set; }
		public IDictionary<string, string> arguments { get; set; }
	}
}