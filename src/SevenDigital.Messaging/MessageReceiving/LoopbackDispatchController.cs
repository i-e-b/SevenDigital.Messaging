namespace SevenDigital.Messaging.MessageReceiving
{
	public class LoopbackDispatchController:IDispatchController
	{

		public IDestinationPoller CreatePoller(string destinationName)
		{
			return null;
		}

		public void Shutdown()
		{
		}
	}
}