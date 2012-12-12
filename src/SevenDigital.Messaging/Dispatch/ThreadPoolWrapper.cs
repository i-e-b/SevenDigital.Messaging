using System;
using System.Threading.Tasks;

namespace SevenDigital.Messaging.Dispatch
{
	public class WorkWrapper : IWorkWrapper
	{
		public void Do(Action action) 
		{
			Task.Factory.StartNew(action);
		}
	}

	public interface IWorkWrapper
	{
		void Do(Action action);
	}
}
