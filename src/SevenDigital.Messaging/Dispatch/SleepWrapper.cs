using System.Threading;

namespace SevenDigital.Messaging.Dispatch
{
	public class SleepWrapper : ISleepWrapper
	{
		public void Sleep(int i)
		{
			Thread.Sleep(i);
		}
	}

	public interface ISleepWrapper
	{
		void Sleep(int i);
	}
}
