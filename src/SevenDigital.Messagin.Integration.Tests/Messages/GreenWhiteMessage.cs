using System;

namespace SevenDigital.Messaging.Integration.Tests.Messages
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
        public string Text { get; private set; }
        public string Text2 { get; private set; }
    }
}