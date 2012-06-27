namespace SevenDigital.Messaging.Core.Domain
{
	public interface IEndpointGenerator
	{
		Endpoint Generate();
	}

	public interface IUniqueEndpointGenerator : IEndpointGenerator {}
	public interface ISenderEndpointGenerator : IEndpointGenerator {}
}