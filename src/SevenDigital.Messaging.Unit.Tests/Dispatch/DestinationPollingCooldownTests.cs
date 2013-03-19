using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class DestinationPollingCooldownTests
	{
		IDestinationPoller subject;
		IMessagingBase messagingBase;
		ISleepWrapper sleepWrapper;
		IMessageDispatcher dispatcher;

		[SetUp]
		public void A_destination_poller_with_a_dispatcher ()
		{
			messagingBase = Substitute.For<IMessagingBase>();
			sleepWrapper = Substitute.For<ISleepWrapper>();

			dispatcher = new FakeDispatcher(2, 1, 0, 0, 0, 0);

			subject = new DestinationPoller(messagingBase, sleepWrapper, dispatcher);
		}

		[Test]
		public void When_stopping_Should_wait_until_handlers_inflight_returns_zero ()
		{
			subject.Stop();

			Assert.That(((FakeDispatcher)dispatcher).HandlersInflightCalls,
				Is.EqualTo(3));
		}

		[Test]
		public void Should_sleep_while_waiting ()
		{
			subject.Stop();

			sleepWrapper.Received(2).SleepMore();
		}

		public class FakeDispatcher : IMessageDispatcher
		{
			public FakeDispatcher(params int[] returnValues)
			{
                HandlersInflightCalls = 0;
				_returnValues = new Stack<int>();
                foreach (var i in returnValues.Reverse()) _returnValues.Push(i);
			}
			public void TryDispatch(IPendingMessage<object> pendingMessage) { }

            public int HandlersInflightCalls;
			readonly Stack<int> _returnValues;
			public int HandlersInflight
			{
				get
				{
                    Interlocked.Increment(ref HandlersInflightCalls);
					return _returnValues.Pop();
				}
			}
			public void AddHandler<TMessage, THandler>() where TMessage : class, IMessage where THandler : IHandle<TMessage> { }
			public void RemoveHandler<T>()
			{
			}

			public int CountHandlers()
			{
				return 0;
			}

			public IEnumerable<Type> Handlers()
			{
				yield break;
			}
		}
	}
}
