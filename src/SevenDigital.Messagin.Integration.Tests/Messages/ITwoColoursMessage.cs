namespace SevenDigital.Messaging.Integration.Tests.Messages
{
    public interface ITwoColoursMessage : IColourMessage
    {
        string Text2 { get; }
    }
}