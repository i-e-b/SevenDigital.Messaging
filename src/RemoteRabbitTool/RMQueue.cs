using System;

namespace RemoteRabbitTool
{
	public class RMQueue
	{
		public int memory { get; set; }
		public DateTime idle_since { get; set; }
		public int messages_ready { get; set; }
		public int messages_unacknowledged { get; set; }
		public int messages { get; set; }
		public int consumers { get; set; }
		public int pending_acks { get; set; }
		public double avg_ingress_rate { get; set; }
		public double avg_egress_rate { get; set; }
		public string name { get; set; }
		public bool durable { get; set; }
		public string vhost { get; set; }
		public string node { get; set; }
	}
}