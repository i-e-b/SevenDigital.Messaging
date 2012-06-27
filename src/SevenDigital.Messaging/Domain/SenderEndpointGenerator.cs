namespace SevenDigital.Messaging.Domain
{
	public class SenderEndpointGenerator:IEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Sender");
		}
	}
}
