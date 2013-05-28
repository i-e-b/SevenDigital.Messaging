using System;

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
	public class RetryMessageAttribute : Attribute
	{
		readonly Type _retryExceptionType;

		/// <summary>
		/// Indicate that the handler should retry any message that fails with the given exception type
		/// </summary>
		/// <param name="retryExceptionType">A type of exception that is automatically recoverable with a retry</param>
		public RetryMessageAttribute(Type retryExceptionType)
		{
			_retryExceptionType = retryExceptionType;
		}

		/// <summary>
		/// A type of exception that is automatically recoverable with a retry
		/// </summary>
		public Type RetryExceptionType
		{
			get { return _retryExceptionType; }
		}
	}
}