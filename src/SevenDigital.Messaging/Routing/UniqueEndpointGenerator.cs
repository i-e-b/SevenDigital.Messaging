namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Auto generator that produces unique name for a given installation of a service
	/// </summary>
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		readonly string strongName;

		/// <summary>
		/// Create a new endpoint generator.
		/// This will create a name based on the host machine and the calling assembly
		/// </summary>
		public UniqueEndpointGenerator()
		{
			strongName =  
				Naming.MachineName()
				+ "_" 
				+ Naming.GoodAssemblyName()
				+ "_Listener";
		}

		/// <summary>
		/// Generate the endpoint name
		/// </summary>
		public Endpoint Generate()
		{
			return new Endpoint(strongName);
		}
	}
}