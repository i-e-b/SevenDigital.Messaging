namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Contract for a routing endpoint
	/// </summary>
	public interface IRoutingEndpoint
	{
		/// <summary>
		/// Return a string name for a routing endpoint
		/// </summary>
		string ToString();
	}
}