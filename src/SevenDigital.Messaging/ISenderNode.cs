namespace SevenDigital.Messaging
{
	/// <summary>
	/// A messaging node that can send messages
	/// </summary>
	public interface ISenderNode
	{
		/// <summary>
		/// Send the given message. Does not guarantee reception.
		/// </summary>
		/// <param name="message">Message to be send. This must be a serialisable type</param>
		void SendMessage<T>(T message) where T : class, IMessage;
	}
}