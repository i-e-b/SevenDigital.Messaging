using System;

namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Auto generator that produces unique name for a given installation of a service
	/// </summary>
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		readonly string strongName;
		readonly string integrationName;

		/// <summary>
		/// Create a new endpoint generator.
		/// This will create a name based on the host machine and the calling assembly
		/// </summary>
		public UniqueEndpointGenerator()
		{
			UseIntegrationTestName = false;

			strongName =  
				Naming.MachineName()
				+ "_" 
				+ Naming.GoodAssemblyName()
				+ "_Listener";
			
			integrationName = "Test_Listener_" + Guid.NewGuid();
		}

		/// <summary>
		/// Generate the endpoint name
		/// </summary>
		public Endpoint Generate()
		{
			return new Endpoint(
				UseIntegrationTestName
				? integrationName
				: strongName);
		}

		/// <summary>
		/// If true, will generate names that integration mode will delete.
		/// </summary>
		public bool UseIntegrationTestName { get; set; }
	}
}