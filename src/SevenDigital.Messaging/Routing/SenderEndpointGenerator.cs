namespace SevenDigital.Messaging.Routing
{
	public class SenderEndpointGenerator:ISenderEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Sender");
		}
	}
}
