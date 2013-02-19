using System;
using System.IO;
using System.Net;

namespace SevenDigital.Messaging
{
    /// <summary>
    /// <para>Apply to classes of type IHandle&lt;T&gt;</para>
    /// <para> - If the handler throws an exception of the given type,
    /// the triggering message will be returned to the queue to be reprocessed.</para>
    /// <para> - If the handler exits normally, or throws a non-matching exception, the message
    /// will be removed from the queue.</para>
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RetryMessageAttribute:Attribute
	{
		readonly Type _retryExceptionType;

		public RetryMessageAttribute(Type retryExceptionType)
		{
			_retryExceptionType = retryExceptionType;
		}

		public Type RetryExceptionType
		{
			get { return _retryExceptionType; }
		}
	}


    [RetryMessage(typeof(IOException))]
    [RetryMessage(typeof(WebException))]
	class Sample : IHandle<IMessage>
	{
		public void Handle(IMessage message)
		{
            var rnd = new Random();
            var retryMessage = rnd.Next() % 2 == 0;

			if (retryMessage)
			{
				throw new IOException();
			}
			else
			{
				throw new InvalidOperationException();
            }
		}
	}
}