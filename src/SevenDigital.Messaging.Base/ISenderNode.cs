using SevenDigital.Messaging.MessageSending.Loopback;

namespace SevenDigital.Messaging
{
	public interface ISenderNode
	{
		void SendMessage<T>(T message) where T : class, IMessage;
	}
}