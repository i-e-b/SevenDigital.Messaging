using System;
using NUnit.Framework;
using SevenDigital.Messaging.Dispatch;

namespace SevenDigital.Messaging.Unit.Tests.Dispatch
{
	[TestFixture]
	public class MessageDispatcherTests
	{
		IMessageDispatcher subject;
		Action<ITestMessage> testHandler;
		Action<ITestMessage> anotherHandler;
		Action<IDifferentTypeMessage> aDifferentType;
		IThreadPoolWrapper mockThreadPool;

		volatile int testHandlerHits;
		volatile int anotherHandlerHits;
		volatile int aDifferentHits;

		[SetUp]
		public void A_message_dispatcher ()
		{
			mockThreadPool = new FakeThreadPool();
			subject = new MessageDispatcher(mockThreadPool);
			testHandler = msg => { testHandlerHits++; };
			anotherHandler = msg => { anotherHandlerHits++; };
			aDifferentType = msg => { aDifferentHits++; };
		}

		[Test]
		public void When_adding_handler_should_have_that_handler_registered ()
		{
			subject.AddHandler(testHandler);
			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] {testHandler} ));
		}

		[Test]
		public void When_adding_more_than_one_handler_of_a_given_type_should_have_all_registered ()
		{
			subject.AddHandler(testHandler);
			subject.AddHandler(anotherHandler);
			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] {testHandler, anotherHandler} ));
		}

		[Test]
		public void When_adding_different_types_of_handler_they_should_not_be_registered_together ()
		{
			subject.AddHandler(testHandler);
			subject.AddHandler(anotherHandler);
			subject.AddHandler(aDifferentType);

			Assert.That(((MessageDispatcher)subject).HandlersForType<ITestMessage>(), 
				Is.EquivalentTo( new [] {testHandler, anotherHandler} ));

			Assert.That(((MessageDispatcher)subject).HandlersForType<IDifferentTypeMessage>(), 
				Is.EquivalentTo( new [] {aDifferentType} ));
		}

		[Test]
		public void When_dispatching_a_message_should_send_all_matching_handlers_to_thread_pool ()
		{
			testHandlerHits = anotherHandlerHits = aDifferentHits = 0;
			subject.AddHandler(testHandler);
			subject.AddHandler(anotherHandler);
			subject.AddHandler(aDifferentType);

			subject.TryDispatch(new FakeMessage());

			Assert.That(testHandlerHits, Is.EqualTo(1));
			Assert.That(anotherHandlerHits, Is.EqualTo(1));
			Assert.That(aDifferentHits, Is.EqualTo(0));
		}
		
		[Test]
		public void When_dispatching_a_super_class_message_should_send_all_matching_handlers_to_thread_pool ()
		{
			testHandlerHits = anotherHandlerHits = aDifferentHits = 0;
			subject.AddHandler(testHandler);
			subject.AddHandler(anotherHandler);
			subject.AddHandler(aDifferentType);

			subject.TryDispatch(new SuperMessage());

			Assert.That(testHandlerHits, Is.EqualTo(1));
			Assert.That(anotherHandlerHits, Is.EqualTo(1));
			Assert.That(aDifferentHits, Is.EqualTo(0));
		}
		
	}

	public class FakeThreadPool : IThreadPoolWrapper
	{
		public void Do(Action action)
		{
			action();
		}

		public bool IsThreadAvailable()
		{
			return true;
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
	
	interface ITestMessage:IMessage
	{
	}
	interface ISuperTestMessage:ITestMessage
	{
	}
	interface IDifferentTypeMessage:IMessage
	{
	}
}
