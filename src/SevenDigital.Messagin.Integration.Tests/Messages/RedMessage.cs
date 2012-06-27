using System;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public class RedMessage : IColourMessage {

		public RedMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId {get; set;}

		public string Text
		{
			get { return "Red"; }
		}
	}
}