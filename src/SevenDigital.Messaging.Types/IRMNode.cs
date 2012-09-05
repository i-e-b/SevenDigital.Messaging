namespace SevenDigital.Messaging
{
	public interface IRMNode
	{
		string name { get; set; }
		string type { get; set; }
		bool running { get; set; }
		string os_pid { get; set; }
		long mem_used { get; set; }
		long mem_limit { get; set; }
		bool mem_alarm { get; set; }
		long disk_free { get; set; }
		long disk_free_limit { get; set; }
		bool disk_free_alarm { get; set; }
		long uptime { get; set; }
		long run_queue { get; set; }
		int processors { get; set; }
		long mem_ets { get; set; }
		long mem_binary { get; set; }
		long mem_proc { get; set; }
		long mem_proc_used { get; set; }
		long mem_atom { get; set; }
		long mem_atom_used { get; set; }
		long mem_code { get; set; }
		string fd_used { get; set; }
		int fd_total { get; set; }
		int sockets_used { get; set; }
		int sockets_total { get; set; }
		long proc_used { get; set; }
		long proc_total { get; set; }
		string statistics_level { get; set; }
		string erlang_version { get; set; }
		bool AnyAlarms();
		string FreeMemPercent();
		string FreeDisk();
	}
}