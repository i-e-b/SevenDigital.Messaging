using SevenDigital.Messaging.Types;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public interface IColourMessage : IMessage
	{
		string Text { get; }
	}
}
