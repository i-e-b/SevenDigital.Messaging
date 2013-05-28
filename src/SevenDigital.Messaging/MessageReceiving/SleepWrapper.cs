using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Thread sleep implementation of sleeping
	/// </summary>
	public class SleepWrapper : ISleepWrapper
	{
		int _sleep;
		
		int BurstSleep()
		{
			if (_sleep < 255) return (_sleep * 2) + 1;
			return 255;
		}

		/// <summary>
		/// Reset sleep duration
		/// </summary>
		public void Reset()
		{
			_sleep = 0;
		}

		/// <summary>
		/// Sleep and increment duration
		/// </summary>
		public void SleepMore()
		{
			Thread.Sleep(BurstSleep());
		}
	}

	/// <summary>
	/// Injection wrapper for variable duration waiting
	/// </summary>
	public interface ISleepWrapper
	{
		/// <summary>
		/// Reset sleep duration
		/// </summary>
		void Reset();

		/// <summary>
		/// Sleep and increment duration
		/// </summary>
		void SleepMore();
	}
}
