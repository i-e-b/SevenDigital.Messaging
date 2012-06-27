using System;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public class JokerMessage : IVillainMessage
	{
		public JokerMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId {get; set;}
		public string Text
		{
			get { return "Superman"; }
		}
	}
}