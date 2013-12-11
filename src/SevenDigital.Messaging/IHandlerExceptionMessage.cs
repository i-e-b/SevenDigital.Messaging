using System;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Message sent by some event hooks when a handler throws an exception
	/// </summary>
	public interface IHandlerExceptionMessage : IMessage
	{
		/// <summary>
		/// Date time that exception was thrown
		/// </summary>
		DateTime Date { get; set; }

		/// <summary>
		/// Original message that triggered the exception
		/// </summary>
		IMessage CausingMessage { get; set; }

		/// <summary>
		/// Exception type and message
		/// </summary>
		string Exception { get; set; }

		/// <summary>
		/// Type name of handler that threw the exception
		/// </summary>
		string HandlerTypeName { get; set; }
	}
}