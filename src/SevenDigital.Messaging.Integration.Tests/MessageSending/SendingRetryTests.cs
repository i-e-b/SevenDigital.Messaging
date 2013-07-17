using System;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Serialisation;
using SevenDigital.Messaging.MessageReceiving;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
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
			_failingMessagingBase.PrepareForSend(Arg.Any<object>())
				.Returns(new PreparedMessage("",""));
			_failingMessagingBase.When(m=>m.SendPrepared(Arg.Any<IPreparedMessage>()))
				.Do(c=> { throw new Exception("test exception"); });

			ObjectFactory.Configure(map => map.For<IMessagingBase>().Use(_failingMessagingBase));

			_subject = MessagingSystem.Sender();
		}

		[Test]
		public void when_messaging_cant_connect_to_rabbit_mq_it_will_keep_trying_until_shutdown ()
		{
			_subject.SendMessage(new TestMessage());

			Thread.Sleep(1000);

			var calls = _failingMessagingBase.ReceivedCalls().Count(c => c.GetMethodInfo().Name == "SendPrepared");
			var sleepTime = ((SleepWrapper) ObjectFactory.GetInstance<ISleepWrapper>()).BurstSleep();

			Assert.That(calls, Is.GreaterThan(5), "Sending calls");
			Assert.That(sleepTime, Is.EqualTo(255), "sleeps");
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.SetShutdownTimeout(TimeSpan.Zero);
			MessagingSystem.Control.Shutdown();
		}

		public class TestMessage:IMessage { public Guid CorrelationId { get; set; } }
	}
}