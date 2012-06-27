using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Core
{
	public interface IHandle<T> where T : IMessage
	{
		void Handle(T message);
	}
}