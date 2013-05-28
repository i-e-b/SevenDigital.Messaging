using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.TestingEventHook
{
	[TestFixture]
	public class TestingEventHookStoresEvents
	{
		ITestEventHook subject;
		IList<IMessage> expected_sent_messages;
		IList<IMessage> expected_received_messages;
		IList<Exception> expected_handler_exceptions;

		[SetUp]
		public void With_loopback_messaging_and_the_testing_event_hook_added ()
		{
			Messaging.Configure.WithLoopbackMode();
			Messaging.Testing.LoopbackEvents().Reset();

			subject = ObjectFactory.GetInstance<ITestEventHook>();
			
			expected_sent_messages = new List<IMessage>{
				new LevelOne(), new LevelOne(),
				new LevelTwo(), new LevelTwo(),
				new LevelThree{TheException = new Exception("Hello")},
				new LevelThree{TheException = new Exception("World")}
			};
			expected_received_messages = expected_sent_messages.Where(m => m is ILevelTwo).ToList();
			expected_handler_exceptions = expected_sent_messages.OfType<ILevelThree>().Select(m=>m.TheException).ToList();

			var listener = Messaging.Receiver().Listen();
			var sender = Messaging.Sender();

			// add handler for subset of messages
			listener.Handle<ILevelTwo>().With<L2Handler>();
			
			// add handler that will throw for subset of messages
			listener.Handle<ILevelThree>().With<FailingHandler>();
			
			// send some messages
			foreach (var message in expected_sent_messages)
			{
				sender.SendMessage(message);
			}
		}

		[Test]
		public void Should_have_a_list_of_sent_messages ()
		{
			Assert.That(subject.SentMessages, Is.EquivalentTo(expected_sent_messages));
		}

		[Test]
		public void	Should_have_a_list_of_received_messages ()
		{
			Assert.That(subject.ReceivedMessages, Is.EquivalentTo(expected_received_messages));
		}

		[Test]
		public void Should_have_a_list_of_handler_exceptions ()
		{
			Assert.That(subject.HandlerExceptions, Is.EquivalentTo(expected_handler_exceptions));
		}

		[Test]
		public void Should_have_no_sent_messages_after_a_reset ()
		{
			subject.Reset();
			Assert.That(subject.SentMessages, Is.Empty);
		}
		[Test]
		public void Should_have_no_received_messages_after_a_reset ()
		{
			subject.Reset();
			Assert.That(subject.ReceivedMessages, Is.Empty);
		}
		[Test]
		public void Should_have_no_handler_exceptions_after_a_reset ()
		{
			subject.Reset();
			Assert.That(subject.HandlerExceptions, Is.Empty);
		}
	}

	public class FailingHandler:IHandle<ILevelThree>
	{
		public void Handle(ILevelThree message)
		{
			throw message.TheException;
		}
	}

	public class L2Handler:IHandle<ILevelTwo>
	{
		public void Handle(ILevelTwo message)
		{
		}
	}
	
	public interface ILevelOne:IMessage
	{
	}

	class LevelOne : ILevelOne
	{
		public LevelOne()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
	}

	public interface ILevelThree:IMessage
	{
		Exception TheException { get; set; }
	}

	class LevelThree : ILevelThree
	{
		public LevelThree()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
		public Exception TheException { get; set; }
	}

	public interface ILevelTwo:IMessage
	{
	}

	class LevelTwo : ILevelTwo
	{
		public LevelTwo()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
	}
}
