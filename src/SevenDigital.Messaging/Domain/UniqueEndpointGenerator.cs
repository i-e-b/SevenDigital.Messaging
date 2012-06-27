using System;

namespace SevenDigital.Messaging.Core.Domain
{
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint(Environment.MachineName + Guid.NewGuid());
		}
	}
}