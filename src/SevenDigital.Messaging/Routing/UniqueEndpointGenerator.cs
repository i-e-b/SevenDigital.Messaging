using System;

namespace SevenDigital.Messaging.Routing
{
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint(Environment.MachineName + Guid.NewGuid());
		}
	}
}