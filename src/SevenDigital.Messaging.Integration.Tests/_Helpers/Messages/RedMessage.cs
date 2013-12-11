using System;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Messages
{
	public class RedMessage : IColourMessage
	{

		public RedMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }

		public string Text
		{
			get { return "Red"; }
			set { }
		}
	}
}