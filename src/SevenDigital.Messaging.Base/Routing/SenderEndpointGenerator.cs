using System;

namespace SevenDigital.Messaging.Routing
{
	public class SenderEndpointGenerator:ISenderEndpointGenerator
	{
		readonly string strongName;

		public SenderEndpointGenerator()
		{
			strongName =  
				Naming.MachineName()
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
