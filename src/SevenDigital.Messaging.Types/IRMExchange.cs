using System.Collections.Generic;

namespace SevenDigital.Messaging
{
	public interface IRMExchange
	{
		string name { get; set; }
		string vhost { get; set; }
		string type { get; set; }
		bool durable { get; set; }
		bool auto_delete { get; set; }
		bool @internal { get; set; }
		IDictionary<string, string> arguments { get; set; }
	}
}