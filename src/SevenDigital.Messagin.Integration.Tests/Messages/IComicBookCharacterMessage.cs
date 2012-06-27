using SevenDigital.Messaging.Types;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public interface IComicBookCharacterMessage : IMessage
	{
		string Text { get; }
	}
}