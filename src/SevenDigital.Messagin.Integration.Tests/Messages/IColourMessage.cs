using SevenDigital.Messaging.Types;

namespace SevenDigital.Messaging.Integration.Tests.Messages
{
	public interface IColourMessage : IMessage
	{
		string Text { get; }
	}
}
