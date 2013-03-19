namespace SevenDigital.Messaging.Routing
{
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		readonly string strongName;

		public UniqueEndpointGenerator()
		{
			strongName =  
				Naming.MachineName()
				+ "_" 
				+ Naming.GoodAssemblyName()
				+ "_Listener";
		}

		public Endpoint Generate()
		{
			return new Endpoint(strongName);
		}
	}
}