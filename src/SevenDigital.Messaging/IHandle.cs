namespace SevenDigital.Messaging
{
	/// <summary>
	/// Handler contract. All classes which receive messages must implement this.
	/// A class may implement IHandle for more than one message type
	/// </summary>
	/// <typeparam name="T">Type of message handled</typeparam>
	public interface IHandle<T> where T : IMessage
	{
		/// <summary>
		/// Handle the given message
		/// </summary>
		void Handle(T message);
	}
}