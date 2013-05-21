using System;

namespace SevenDigital.Messaging
{
	public interface IMessage {
		Guid CorrelationId { get; set; }
	}
}
