namespace SevenDigital.Messaging.Integration.Tests._Helpers.Messages
{
    public interface ITwoColoursMessage : IColourMessage
    {
        string Text2 { get; }
    }
}