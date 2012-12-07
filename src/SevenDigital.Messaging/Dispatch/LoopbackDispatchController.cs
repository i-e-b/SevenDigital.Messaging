namespace SevenDigital.Messaging.Dispatch
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