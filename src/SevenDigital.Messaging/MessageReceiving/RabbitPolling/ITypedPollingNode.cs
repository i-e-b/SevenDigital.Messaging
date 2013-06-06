using System;
using DispatchSharp;
using SevenDigital.Messaging.Base;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// A work queue that polls for message types
	/// </summary>
	public interface ITypedPollingNode : IWorkQueue<IPendingMessage<object>>
	{
		/// <summary>
		/// A message type for which to poll
		/// </summary>
		void AddMessageType(Type type);
	}
}