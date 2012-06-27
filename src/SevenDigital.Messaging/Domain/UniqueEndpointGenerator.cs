using System;

namespace SevenDigital.Messaging.Domain
{
	public class UniqueEndpointGenerator : IEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint(Guid.NewGuid().ToString());
		}
	}
}