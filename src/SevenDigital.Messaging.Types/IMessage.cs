using System;

namespace SevenDigital.Messaging.Types
{
	public interface IMessage {
		Guid CorrelationId { get; }
	}
}
