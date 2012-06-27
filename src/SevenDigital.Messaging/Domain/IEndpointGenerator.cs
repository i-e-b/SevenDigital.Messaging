namespace SevenDigital.Messaging.Domain
{
	public interface IEndpointGenerator
	{
		Endpoint Generate();
	}

	public interface IUniqueEndpointGenerator : IEndpointGenerator {}
	public interface ISenderEndpointGenerator : IEndpointGenerator {}
}