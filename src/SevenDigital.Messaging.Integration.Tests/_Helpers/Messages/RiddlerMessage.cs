using System;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Messages
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