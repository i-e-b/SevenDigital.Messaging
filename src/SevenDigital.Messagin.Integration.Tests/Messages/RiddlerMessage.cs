using System;

namespace SevenDigital.Jester.Delivery.Messaging.Integration.Tests.Messages
{
	public class RiddlerMessage : IVillainMessage
	{
		public RiddlerMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId {get; set;}
		public string Text
		{
			get { return "Batman"; }
		}
	}
}