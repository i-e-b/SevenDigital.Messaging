using System.Diagnostics;
using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class SleepWrapperTests
	{
		ISleepWrapper _subject;

		[SetUp]
		public void setup()
		{
			_subject = new SleepWrapper();
		}

		[Test]
		public void the_sleeper_sleeps ()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_subject.SleepMore();
			_subject.SleepMore();
			_subject.SleepMore();
			_subject.SleepMore();

			stopwatch.Stop();
			Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(26));
		}

		[Test]
		public void the_sleeper_must_awaken ()
		{
			int slumber = 0;
			for (int i = 0; i < 1000; i++)
			{
				slumber = ((SleepWrapper)_subject).BurstSleep();
			}

			Assert.That(slumber, Is.EqualTo(255));

			_subject.Reset();
			Assert.That(((SleepWrapper)_subject).BurstSleep(), Is.EqualTo(1));
		}
	}
}