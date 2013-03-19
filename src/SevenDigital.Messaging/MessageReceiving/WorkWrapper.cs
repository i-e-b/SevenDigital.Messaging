using System;
using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
{
	public class WorkWrapper : IWorkWrapper
	{
		public void Do(Action action) 
		{
			new Thread(() => action()) {
				IsBackground = true,
				Name = "HandlerAction"
			}.Start();
		}
	}

	public interface IWorkWrapper
	{
		void Do(Action action);
	}
}
