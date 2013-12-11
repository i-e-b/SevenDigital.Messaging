using System;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Messages
{
    public class GreenWhiteMessage : ITwoColoursMessage
    {
        public GreenWhiteMessage()
        {
            Text = "Green";
            Text2 = "White";
            CorrelationId = Guid.NewGuid();
        }
        public Guid CorrelationId { get; set; }
        public string Text { get; set; }
        public string Text2 { get; private set; }
    }
}