using System;

namespace SevenDigital.Messaging.Integration.Tests.Messages
{
	public class SupermanMessage : ISuperHeroMessage
	{
		public Guid CorrelationId {get; set;}

		public SupermanMessage()
		{
			CorrelationId = Guid.NewGuid();
		}

		public string Text
		{
			get { return "Superman"; }
		}
	}

	public class BatmanMessage : ISuperHeroMessage
	{
		public Guid CorrelationId {get; set;}

		public BatmanMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public string Text
		{
			get { return "Batman"; }
		}
	}

	public interface ISuperHeroMessage : IComicBookCharacterMessage
	{
	}
}