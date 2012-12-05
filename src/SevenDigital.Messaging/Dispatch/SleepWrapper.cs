using System.Threading;

namespace SevenDigital.Messaging.Dispatch
{
	public class SleepWrapper : ISleepWrapper
	{
		public void Sleep()
		{
			Thread.Sleep(250);
		}
	}

	public interface ISleepWrapper
	{
		void Sleep();
	}
}
