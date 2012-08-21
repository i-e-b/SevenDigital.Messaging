using System;

namespace SevenDigital.Messaging.Routing
{
	public class SenderEndpointGenerator:ISenderEndpointGenerator
	{
		string strongName;

		public SenderEndpointGenerator()
		{
			strongName =  
				Environment.MachineName 
				+ "_" 
				+ Naming.GoodAssemblyName()
				+ "_Sender";
		}
		public Endpoint Generate()
		{
			return new Endpoint(strongName);
		}
	}
}
