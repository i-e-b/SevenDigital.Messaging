using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.MessageReceiving;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class MessageHandlerTests
	{
		IHandlerManager _subject;
		Type[] _types;
		Type[] _handlers;
		IPendingMessage<object> _pendingSubMessage, _pendingMessageOne, _pendingIMessage;
		IMessagingBase _messageBase;
		bool FinishCalled, CancelCalled, LogWritten;
		IEventHook _eventHook;
		ISleepWrapper _sleepWrapper;

		[SetUp]
		public void setup()
		{
			_sleepWrapper = Substitute.For<ISleepWrapper>();
			_messageBase = Substitute.For<IMessagingBase>();
			ObjectFactory.Configure(map=>map.For<IMessagingBase>().Use(_messageBase));

			FinishCalled = CancelCalled = LogWritten = false;
			_subject = new HandlerManager(_sleepWrapper);
			_types = new[] {typeof (IMessageOne), typeof (IMessageTwo), typeof (IMessageThree)};
			_handlers = new[] {typeof (HandlerOne), typeof (HandlerTwo), typeof (HandlerThree)};

			_pendingSubMessage = Substitute.For<IPendingMessage<ISubMessage>>();
			_pendingSubMessage.Message.Returns(new SubMessage());
			_pendingSubMessage.Finish.Returns(() => { FinishCalled = true;});
			
			_pendingMessageOne = Substitute.For<IPendingMessage<IMessageOne>>();
			_pendingMessageOne.Message.Returns(new MessageOne());
			
			_pendingIMessage = Substitute.For<IPendingMessage<IMessage>>();
			_pendingIMessage.Message.Returns(new BaseMessage());
			_pendingIMessage.Cancel.Returns(() => { CancelCalled = true;});

			MessagingSystem.Events.ClearEventHooks();
			_eventHook = Substitute.For<IEventHook>();
			ObjectFactory.Configure(map=>map.For<IEventHook>().Use(_eventHook));
			MessagingSystem.Control.OnInternalWarning(e => {
				Console.WriteLine(e.Message);
				LogWritten = true;
			});
		}

		[Test]
		public void handler_starts_out_with_no_bindings()
		{
			Assert.That(_subject.CountHandlers(), Is.EqualTo(0));
		}

		[Test]
		public void adding_a_binding_is_reflected_in_the_handler_count()
		{
			for (int i = 0; i < 3; i++)
			{
				_subject.AddHandler(_types[i], _handlers[i]);
				Assert.That(_subject.CountHandlers(), Is.EqualTo(i + 1));
			}
		}

		[Test]
		public void removing_a_binding_is_reflected_in_the_handler_count()
		{
			for (int i = 0; i < 3; i++)
			{
				_subject.AddHandler(_types[i], _handlers[i]);
			}
			for (int i = 2; i >= 0; i--)
			{
				_subject.RemoveHandler(_handlers[i]);
				Assert.That(_subject.CountHandlers(), Is.EqualTo(i));
			}
		}

		[Test]
		public void removed_handlers_are_not_included_in_handler_list()
		{
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));
			_subject.RemoveHandler(typeof (BaseMessageHandler));

			Assert.That(_subject.GetMatchingHandlers(typeof (IMessage)), Is.Empty);
		}

		[Test]
		public void handler_selection_does_not_match_more_generic_types()
		{
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));
			
			Assert.That(_subject.GetMatchingHandlers(typeof (IMessage)), Is.Empty);
		}

		[Test]
		public void handler_selection_matches_more_specific_types()
		{
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));
			
			Assert.That(_subject.GetMatchingHandlers(typeof(ISubMessage)), Is.Not.Empty);
		}

		[Test]
		public void handling_a_message_calls_all_matching_message_handlers()
		{
			SubMessageHandler.Count = 0;
			BaseMessageHandler.Count = 0;
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));

			_subject.TryHandle(_pendingSubMessage);

			Assert.That(SubMessageHandler.Count, Is.EqualTo(1));
			Assert.That(BaseMessageHandler.Count, Is.EqualTo(1));
		}

		[Test]
		public void successfully_handling_a_message_resets_the_sleep_wraper ()
		{
			_sleepWrapper.ClearReceivedCalls();
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));

			_subject.TryHandle(_pendingSubMessage);

			_sleepWrapper.Received().Reset();
		}

		[Test]
		public void handling_a_messaging_does_not_call_handlers_for_other_types()
		{
			HandlerOne.Count = 0;
			HandlerTwo.Count = 0;
			_subject.AddHandler(typeof (IMessageOne), typeof (HandlerOne));
			_subject.AddHandler(typeof (IMessageTwo), typeof (HandlerTwo));

			_subject.TryHandle(_pendingMessageOne);

			Assert.That(HandlerOne.Count, Is.EqualTo(1));
			Assert.That(HandlerTwo.Count, Is.EqualTo(0));
		}

		[Test]
		public void handling_a_message_triggers_more_generic_handlers()
		{
			BaseMessageHandler.Count = 0;
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));

			_subject.TryHandle(_pendingSubMessage);

			Assert.That(BaseMessageHandler.Count, Is.EqualTo(1));
		}

		[Test]
		public void handling_a_message_does_not_trigger_more_specific_handlers()
		{
			SubMessageHandler.Count = 0;
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));

			_subject.TryHandle(_pendingIMessage);

			Assert.That(SubMessageHandler.Count, Is.EqualTo(0));
		}

		[Test]
		public void messages_with_no_handler_are_finished_with_a_log_message ()
		{
			_subject.RemoveHandler(typeof(ISubMessage));
			_subject.TryHandle(_pendingSubMessage);

			Assert.That(FinishCalled);
			Assert.That(LogWritten, Is.True);
		}

		[Test]
		public void messages_that_are_handled_with_no_exceptions_are_finished_with_no_log ()
		{
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));
			_subject.TryHandle(_pendingSubMessage);

			Assert.That(FinishCalled, "finish didn't call");
			Assert.That(LogWritten, Is.False, "a warning was written");
		}

		[Test]
		public void messages_that_throw_exceptions_marked_as_retry_are_cancelled()
		{
			_subject.AddHandler(typeof (IMessage), typeof (IoRetryHandler));
			_subject.TryHandle(_pendingIMessage);

			Assert.That(CancelCalled);
		}

		[Test]
		public void messages_that_throw_exceptions_not_marked_as_retry_are_finished ()
		{
			_subject.AddHandler(typeof (IMessage), typeof (FailingHandler));
			_subject.TryHandle(_pendingSubMessage);

			Assert.That(FinishCalled);
		}

		[Test]
		public void when_a_message_is_handled_with_no_exceptions_the_message_received_hook_is_fired ()
		{
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));
			_subject.TryHandle(_pendingIMessage);

			_eventHook.Received().MessageReceived(Arg.Any<IMessage>());

			_eventHook.DidNotReceive().MessageSent(Arg.Any<IMessage>());
			_eventHook.DidNotReceive().HandlerFailed(Arg.Any<IMessage>(), Arg.Any<Type>(), Arg.Any<Exception>());
		}

		[Test]
		public void when_a_message_is_handled_with_an_exception_the_handler_failed_hook_is_fired ()
		{
			_subject.AddHandler(typeof (IMessage), typeof (FailingHandler));
			_subject.TryHandle(_pendingIMessage);

			_eventHook.Received().HandlerFailed(Arg.Any<IMessage>(), Arg.Any<Type>(), Arg.Any<Exception>());

			_eventHook.DidNotReceive().MessageReceived(Arg.Any<IMessage>());
			_eventHook.DidNotReceive().MessageSent(Arg.Any<IMessage>());
		}

		[Test]
		public void retry_decoration_is_correctly_interpreted_read_for_exactly_matching_types()
		{
			Assert.True(HandlerManager.ShouldRetry(typeof(IOException), typeof(IoRetryHandler)));
		}
		[Test]
		public void retry_is_false_for_base_type_exceptions()
		{
			Assert.False(HandlerManager.ShouldRetry(typeof(Exception), typeof(IoRetryHandler)));
		}
		[Test]
		public void retry_is_false_for_child_type_exceptions()
		{
			Assert.False(HandlerManager.ShouldRetry(typeof(SubIOException), typeof(IoRetryHandler)));
		}

		#region Type junk
		public interface IMessageOne:IMessage { }
		public interface IMessageTwo:IMessage { }
		public interface IMessageThree:IMessage { }

		public interface ISubMessage : IMessageOne { }
		
		public class BaseMessage : IMessage { public Guid CorrelationId { get; set; } }
		public class MessageOne : IMessageOne { public Guid CorrelationId { get; set; } }
		public class SubMessage : ISubMessage { public Guid CorrelationId { get; set; } }

		public class SubMessageHandler : IHandle<ISubMessage> { public static int Count = 0;public void Handle(ISubMessage message) {Count++; } }

		public class BaseMessageHandler:IHandle<IMessage> { public static int Count = 0;public void Handle(IMessage message) {Count++; } }
		public class HandlerOne:IHandle<IMessageOne> {  public static int Count = 0;public void Handle(IMessageOne message) {Count++; } }
		public class HandlerTwo:IHandle<IMessageTwo> {  public static int Count = 0;public void Handle(IMessageTwo message) {Count++; } }
		public class HandlerThree:IHandle<IMessageThree> { public void Handle(IMessageThree message) { } }

		[RetryMessage(typeof(IOException))]
		public class IoRetryHandler : IHandle<IMessage> { public void Handle(IMessage message) { throw new IOException();} }
		
		public class FailingHandler : IHandle<IMessage> { public void Handle(IMessage message) { throw new IOException();} }

		public class SubIOException : IOException { }

		#endregion
	}
}