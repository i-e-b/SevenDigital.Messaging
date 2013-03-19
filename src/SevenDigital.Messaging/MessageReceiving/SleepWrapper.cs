using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
{
	public class SleepWrapper : ISleepWrapper
	{
		int _sleep;
		
		int BurstSleep()
		{
			if (_sleep < 255) return (_sleep * 2) + 1;
			return 255;
		}

		public void Reset()
		{
			_sleep = 0;
		}

		public void SleepMore()
		{
			Thread.Sleep(BurstSleep());
		}
	}

	public interface ISleepWrapper
	{
		void Reset();
		void SleepMore();
	}
}
