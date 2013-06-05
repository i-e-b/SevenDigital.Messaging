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

		/// <summary>
		/// If true, will generate names that integration mode will delete.
		/// </summary>
		bool UseIntegrationTestName { get; set; }
	}
}