namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILoopbackReceiver
	{
		/// <summary>
		/// Send a message
		/// </summary>
		void Send<T>(T message) where T : IMessage;
	}
}