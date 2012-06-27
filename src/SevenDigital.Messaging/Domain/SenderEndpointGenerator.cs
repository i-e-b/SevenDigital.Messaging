namespace SevenDigital.Messaging.Domain
{
	public class SenderEndpointGenerator:ISenderEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Sender");
		}
	}
}
