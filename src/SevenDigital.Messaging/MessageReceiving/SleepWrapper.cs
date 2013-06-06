using System.Threading;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Thread sleep implementation of sleeping
	/// </summary>
	public class SleepWrapper : ISleepWrapper
	{
		int _sleep;
		
		/// <summary>
		/// Increase sleep duration, returning the new duration
		/// </summary>
		public int BurstSleep()
		{
			_sleep = (_sleep < 255) ? (_sleep * 2) + 1 : 255;
			return _sleep;
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
