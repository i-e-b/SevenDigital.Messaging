namespace SevenDigital.Messaging.Core.Domain
{
	public class SenderEndpointGenerator:ISenderEndpointGenerator
	{
		public Endpoint Generate()
		{
			return new Endpoint("Sender");
		}
	}
}
