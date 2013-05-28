using System;
using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Background thread action
	/// </summary>
	public class WorkWrapper : IWorkWrapper
	{
		/// <summary>
		/// Perform an action
		/// </summary>
		public void Do(Action action) 
		{
			new Thread(() => action()) {
				IsBackground = true,
				Name = "HandlerAction"
			}.Start();
		}
	}

	/// <summary>
	/// Injection point for performing an action
	/// </summary>
	public interface IWorkWrapper
	{
		/// <summary>
		/// Perform an action
		/// </summary>
		void Do(Action action);
	}
}
