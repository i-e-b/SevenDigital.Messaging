using System;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class MessageDispatcherTests
	{
		IMessageDispatcher subject;
		IWorkWrapper mockWork;

		[SetUp]
		public void A_message_dispatcher ()
		{
			mockWork = new FakeWork();
			subject = new MessageDispatcher(mockWork);
		}

		[Test]
		public void When_adding_handler_should_have_that_handler_registered ()
		{
			subject.AddHandler<ITestMessage, TestMessageHandler>();
			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] { typeof(TestMessageHandler) } ));
		}

		[Test]
		public void When_adding_more_than_one_handler_of_a_given_type_should_have_all_registered ()
		{
			subject.AddHandler<ITestMessage, TestMessageHandler>();
			subject.AddHandler<ITestMessage, AnotherTestMessageHandler>();
			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] {typeof(TestMessageHandler), typeof(AnotherTestMessageHandler)} ));
		}

		[Test]
		public void When_adding_different_types_of_handler_they_should_not_be_registered_together ()
		{
			subject.AddHandler<ITestMessage, TestMessageHandler>();
			subject.AddHandler<ITestMessage, AnotherTestMessageHandler>();
			subject.AddHandler<IDifferentTypeMessage, DifferentTestMessageHandler>();

			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] {typeof(TestMessageHandler), typeof(AnotherTestMessageHandler)} ));

			Assert.That(((MessageDispatcher)subject).HandlersForType<IDifferentTypeMessage>(), 
				Is.EquivalentTo( new [] {typeof(DifferentTestMessageHandler)} ));
		}

		[Test]
		public void When_dispatching_a_message_should_send_all_matching_handlers_to_thread_pool ()
		{
			TestMessageHandler.Hits = AnotherTestMessageHandler.Hits = DifferentTestMessageHandler.Hits = 0;
			subject.AddHandler<ITestMessage, TestMessageHandler>();
			subject.AddHandler<ITestMessage, AnotherTestMessageHandler>();
			subject.AddHandler<IDifferentTypeMessage, DifferentTestMessageHandler>();

			subject.TryDispatch(Wrap(new FakeMessage()));

			Assert.That(TestMessageHandler.Hits, Is.EqualTo(1));
			Assert.That(AnotherTestMessageHandler.Hits, Is.EqualTo(1));
			Assert.That(DifferentTestMessageHandler.Hits, Is.EqualTo(0));
		}
		
		[Test]
		public void When_dispatching_a_super_class_message_should_send_all_matching_handlers_to_thread_pool ()
		{
			TestMessageHandler.Hits = AnotherTestMessageHandler.Hits = DifferentTestMessageHandler.Hits = 0;
			subject.AddHandler<ITestMessage, TestMessageHandler>();
			subject.AddHandler<ITestMessage, AnotherTestMessageHandler>();
			subject.AddHandler<IDifferentTypeMessage, DifferentTestMessageHandler>();

			subject.TryDispatch(Wrap(new SuperMessage()));
			
			Assert.That(TestMessageHandler.Hits, Is.EqualTo(1));
			Assert.That(AnotherTestMessageHandler.Hits, Is.EqualTo(1));
			Assert.That(DifferentTestMessageHandler.Hits, Is.EqualTo(0));
		}

		[Test]
		public void If_a_handler_fails_AND_an_event_hook_fails_when_reporting_it_then_should_finish_or_cancel_the_message ()
		{
			subject.AddHandler<ITestMessage, FailingMessageHandler>();

			int cancelCount = 0;
			int finishCount = 0;
			var pendingMessage =new PendingMessage<ITestMessage> {
                Message = new FakeMessage(),
				Cancel = () => { cancelCount++; },
                Finish = () => { finishCount++; }
			};
			subject.TryDispatch(pendingMessage);
			subject.TryDispatch(pendingMessage);

			Assert.That(finishCount, Is.EqualTo(1));
			Assert.That(cancelCount, Is.EqualTo(1));
		}

		[RetryMessage(typeof(ArgumentException))]
		public class FailingMessageHandler:IHandle<ITestMessage>
		{
			public static int Hits = 0;
			public void Handle(ITestMessage message)
			{
				Hits++;
				if (Hits == 1) throw new ArgumentException();
				throw new Exception();
			}
		}

		IPendingMessage<T> Wrap<T>(T message)
        {
            return new PendingMessage<T> {
                Message = message,
				Cancel = () => { },
                Finish = () => { }
			};
		}

	}

	public class TestMessageHandler : IHandle<ITestMessage>
	{
		public static int Hits = 0;
		public void Handle(ITestMessage message) { Hits++; }
	}
	public class AnotherTestMessageHandler : IHandle<ITestMessage>
	{
		public static int Hits = 0;
		public void Handle(ITestMessage message) { Hits++; }
	}
	public class DifferentTestMessageHandler : IHandle<IDifferentTypeMessage>
	{
		public static int Hits = 0;
		public void Handle(IDifferentTypeMessage message) { Hits++; }
	}

	public class FakeWork : IWorkWrapper
	{
		public void Do(Action action)
		{
			action();
		}
	}

	public class FakeMessage:ITestMessage
	{
		public Guid CorrelationId { get; set; }
	}
	public class SuperMessage:ISuperTestMessage
	{
		public Guid CorrelationId { get; set; }
	}

	public interface ITestMessage:IMessage
	{
	}
	interface ISuperTestMessage:ITestMessage
	{
	}

	public interface IDifferentTypeMessage:IMessage
	{
	}
}
