using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
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
