using System;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public class GreenMessage : IColourMessage {
		public GreenMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId {get; set;}
		public string Text
		{
			get { return "Green"; }
		}
	}
}