namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// General contract for generating the name of a messaging endpoint
	/// </summary>
	public interface IEndpointGenerator
	{
		/// <summary>
		/// Create an endpoint name
		/// </summary>
		Endpoint Generate();
	}

	/// <summary>
	/// Auto generator that produces unique name for a given installation of a service
	/// </summary>
	public interface IUniqueEndpointGenerator : IEndpointGenerator {}
}