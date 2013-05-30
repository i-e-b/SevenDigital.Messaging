namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// No-op dispatcher for loopback
	/// </summary>
	public class LoopbackDispatchController : IDispatchController
	{
		/// <summary>
		/// does nothing
		/// </summary>
		public void AddHandler<TMessage, THandler>(string destinationName) where TMessage : class, IMessage where THandler : IHandle<TMessage>
		{
		}
		
		/// <summary>
		/// does nothing
		/// </summary>
		public void RemoveHandler<T>(string destinationName)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public void Shutdown()
		{
		}
	}
}