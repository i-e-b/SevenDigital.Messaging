using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Services
{
	public interface IHandle<T> where T : IMessage
	{
		void Handle(T message);
	}
}