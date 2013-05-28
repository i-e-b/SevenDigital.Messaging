using System;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Base messaging contract. All messages derive from this
	/// </summary>
	public interface IMessage {
		/// <summary>
		/// Unique ID for the message. Can be used to correlate sent messages with received messages
		/// </summary>
		Guid CorrelationId { get; set; }
	}
}
