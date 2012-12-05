using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatch:IMessageDispatch
	{
		public void Dispose()
		{
		}

		public void SubscribeHandler<T>(Action<T> action)
		{
		}

		public void Publish<T>(T message)
		{
		}
	}
}
