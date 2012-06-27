namespace SevenDigital.Messaging.Routing
{
	public interface IEndpointGenerator
	{
		Endpoint Generate();
	}

	public interface IUniqueEndpointGenerator : IEndpointGenerator {}
	public interface ISenderEndpointGenerator : IEndpointGenerator {}
}