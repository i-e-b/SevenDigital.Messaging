using System;

namespace SevenDigital.Messaging
{
	public interface IRMQueue
	{
		long memory { get; set; }
		DateTime idle_since { get; set; }
		long messages_ready { get; set; }
		long messages_unacknowledged { get; set; }
		long messages { get; set; }
		long consumers { get; set; }
		long pending_acks { get; set; }
		double avg_ingress_rate { get; set; }
		double avg_egress_rate { get; set; }
		string name { get; set; }
		bool durable { get; set; }
		string vhost { get; set; }
		string node { get; set; }
	}
}