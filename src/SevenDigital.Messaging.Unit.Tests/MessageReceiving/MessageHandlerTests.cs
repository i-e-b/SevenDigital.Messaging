using System;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class MessageHandlerTests
	{
		IHandlerManager _subject;
		Type[] _types;
		Type[] _handlers;
		IPendingMessage<object> _pendingSubMessage;

		[SetUp]
		public void setup()
		{
			_subject = new HandlerManager();
			_types = new[] {typeof (IMessageOne), typeof (IMessageTwo), typeof (IMessageThree)};
			_handlers = new[] {typeof (HandlerOne), typeof (HandlerTwo), typeof (HandlerThree)};

			_pendingSubMessage = Substitute.For<IPendingMessage<ISubMessage>>();
			_pendingSubMessage.Message.Returns(new SubMessage());
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
		public void handling_a_messaging_does_not_call_handlers_for_other_types()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void handling_a_message_triggers_more_generic_handlers()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void handling_a_message_does_not_trigger_more_specific_handlers()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void messages_with_no_handler_are_finished ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void messages_that_are_handled_with_no_exceptions_are_finished ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void messages_that_throw_exceptions_marked_as_retry_are_cancelled()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void messages_that_throw_exceptions_not_marked_as_retry_are_finished ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void when_a_message_is_handled_with_no_exceptions_the_message_received_hook_is_fired ()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void when_a_message_is_handled_with_an_exception_the_handler_failed_hook_is_fired ()
		{
			Assert.Inconclusive();
		}

		#region Type junk
		public interface IMessageOne:IMessage { }
		public interface IMessageTwo:IMessage { }
		public interface IMessageThree:IMessage { }

		public interface ISubMessage : IMessageOne { }

		public class SubMessage : ISubMessage { public Guid CorrelationId { get; set; } }

		public class SubMessageHandler : IHandle<ISubMessage> { public static int Count = 0;public void Handle(ISubMessage message) {Count++; } }

		public class BaseMessageHandler:IHandle<IMessage> { public static int Count = 0;public void Handle(IMessage message) {Count++; } }
		public class HandlerOne:IHandle<IMessageOne> {  public static int Count = 0;public void Handle(IMessageOne message) {Count++; } }
		public class HandlerTwo:IHandle<IMessageTwo> {  public static int Count = 0;public void Handle(IMessageTwo message) {Count++; } }
		public class HandlerThree:IHandle<IMessageThree> { public void Handle(IMessageThree message) { } }
		#endregion
	}
}