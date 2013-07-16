using System;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture, Ignore("Needs rebuilding to match store-and-forward semantics")]
	public class SendingRetryTests
	{
		ISenderNode _subject;
		IMessagingBase _failingMessagingBase;

		[SetUp]
		public void setup()
		{
			MessagingSystem.Configure.WithDefaults().SetMessagingServer("reallyNotAServerAtAll").SetIntegrationTestMode();
			MessagingSystem.Events.ClearEventHooks();

			_failingMessagingBase = Substitute.For<IMessagingBase>();
			_failingMessagingBase.When(m=>m.SendMessage(Arg.Any<object>())).Do(c=> { throw new Exception("test exception"); });

			ObjectFactory.Configure(map => map.For<IMessagingBase>().Use(_failingMessagingBase));

			_subject = MessagingSystem.Sender();
		}

		[Test, Ignore("Needs rebuilding to match store-and-forward semantics")]
		public void when_messaging_cant_connect_to_rabbit_mq_it_will_keep_trying_until_shutdown ()
		{
			_subject.SendMessage(new TestMessage());

			Thread.Sleep(1000);

			var calls = _failingMessagingBase.ReceivedCalls().Count(c => c.GetMethodInfo().Name == "SendMessage");
			var sleepTime = ((SleepWrapper) ObjectFactory.GetInstance<ISleepWrapper>()).BurstSleep();

			Assert.That(calls, Is.GreaterThan(5));
			Assert.That(sleepTime, Is.EqualTo(255));
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}

		public class TestMessage:IMessage { public Guid CorrelationId { get; set; } }
	}
}