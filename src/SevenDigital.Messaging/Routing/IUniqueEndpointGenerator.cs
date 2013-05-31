namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Auto generator that produces unique name for a given installation of a service
	/// </summary>
	public interface IUniqueEndpointGenerator {
		/// <summary>
		/// Create an endpoint name
		/// </summary>
		Endpoint Generate();
	}
}