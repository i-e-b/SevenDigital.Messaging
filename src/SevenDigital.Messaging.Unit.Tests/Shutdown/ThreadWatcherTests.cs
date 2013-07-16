using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Infrastructure;

namespace SevenDigital.Messaging.Unit.Tests.Shutdown
{
	[TestFixture]
	public class Creating_a_thread_watcher
	{
		readonly Action _emptyAction = () => { };
		Thread _runningThread;
		volatile bool _running;

		[SetUp]
		public void setup()
		{
			_running = true;
			_runningThread = new Thread(() => {
				while (_running)
				{
					Thread.Sleep(1);
				}
			});
			_runningThread.Start();
		}
		[TearDown]
		public void teardown()
		{
			_running = false;
			_runningThread.Join();
		}

// ReSharper disable ObjectCreationAsStatement
		[Test]
		public void with_a_null_thread_throws_an_exception ()
		{
			var ex = Assert.Throws<ArgumentException>(() => { new ThreadWatcher(null, _emptyAction); });
			Assert.That(ex.Message.ToLowerInvariant(), Contains.Substring("thread to watch must be a valid running thread"));
		}

		[Test]
		public void with_a_stopped_thread_throws_an_exception ()
		{
			var ex = Assert.Throws<ArgumentException>(() => { new ThreadWatcher(new Thread(()=> { }), _emptyAction); });
			Assert.That(ex.Message.ToLowerInvariant(), Contains.Substring("thread to watch must be a valid running thread"));
		}

		[Test]
		public void with_a_null_action_throws_an_exception ()
		{
			var ex = Assert.Throws<ArgumentException>(() => { 
				new ThreadWatcher(_runningThread, null);
			});

			Assert.That(ex.Message.ToLowerInvariant(), Contains.Substring("must provide an action to run"));
		}
// ReSharper restore ObjectCreationAsStatement
	}

	[TestFixture]
	public class Watching_thread_for_end
	{
// ReSharper disable NotAccessedField.Local
		volatile bool _actionCalled;
		Thread _runningThread;
		volatile bool _running;

		ThreadWatcher _subject;
// ReSharper restore NotAccessedField.Local

		[SetUp]
		public void setup()
		{
			_running = true;
			_runningThread = new Thread(() => {
				while (_running)
				{
					Thread.Sleep(1);
				}
			});
			_runningThread.Start();
			_actionCalled = false;

			_subject = new ThreadWatcher(_runningThread, TargetAction);
		}

		void TargetAction()
		{
			_actionCalled = true;
		}

		[TearDown]
		public void teardown()
		{
			_running = false;
			_actionCalled = false;
			_runningThread.Join();
			Thread.Sleep(250);
		}

		[Test] [Repeat(10)]
		public void when_thread_ends_should_call_action ()
		{
			_running = false;
			_runningThread.Join();
			Thread.Sleep(100);

			Assert.That(_actionCalled, Is.True, "action called");
		}
		
		[Test] [Repeat(10)]
		public void when_thread_continues_action_should_not_be_called ()
		{
			Thread.Sleep(100);

			Assert.That(_actionCalled, Is.False, "action called");
		}
	}
}