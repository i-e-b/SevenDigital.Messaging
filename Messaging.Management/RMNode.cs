namespace RemoteRabbitTool
{
	public class RMNode
	{
		public string name { get; set; }
		public string type { get; set; }
		public bool running { get; set; }
		public string os_pid { get; set; }

		public long mem_used { get; set; }
		public long mem_limit { get; set; }
		public bool mem_alarm { get; set; }

		public long disk_free { get; set; }
		public long disk_free_limit { get; set; }
		public bool disk_free_alarm { get; set; }

		public long uptime { get; set; }
		public long run_queue { get; set; }
		public int processors { get; set; }

		#region Details
		public long mem_ets { get; set; }
		public long mem_binary { get; set; }
		public long mem_proc { get; set; }
		public long mem_proc_used { get; set; }
		public long mem_atom { get; set; }
		public long mem_atom_used { get; set; }
		public long mem_code { get; set; }

		public string fd_used { get; set; }
		public int fd_total { get; set; }
		public int sockets_used { get; set; }
		public int sockets_total { get; set; }
		public long proc_used { get; set; }
		public long proc_total { get; set; }
		public string statistics_level { get; set; }
		public string erlang_version { get; set; }
		#endregion

		public bool AnyAlarms()
		{
			return disk_free_alarm || mem_alarm;
		}
	}
}
