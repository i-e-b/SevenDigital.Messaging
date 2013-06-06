using System;
using NUnit.Framework;
using SevenDigital.Messaging.MessageReceiving;

namespace SevenDigital.Messaging.Unit.Tests.MessageReceiving
{
	[TestFixture]
	public class MessageHandlerTests
	{
		IHandlerManager _subject;
		Type[] _types;
		Type[] _handlers;

		[SetUp]
		public void setup()
		{
			_subject = new HandlerManager();
			_types = new[] {typeof (IMessageOne), typeof (IMessageTwo), typeof (IMessageThree)};
			_handlers = new[] {typeof (HandlerOne), typeof (HandlerTwo), typeof (HandlerThree)};
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

			Assert.That(_subject.HandlersForType<IMessage>(), Is.Empty);
		}

		[Test]
		public void handler_selection_does_not_match_more_generic_types()
		{
			_subject.AddHandler(typeof (ISubMessage), typeof (SubMessageHandler));
			
			Assert.That(_subject.HandlersForType<IMessage>(), Is.Empty);
		}

		[Test]
		public void handler_selection_matches_more_specific_types()
		{
			_subject.AddHandler(typeof (IMessage), typeof (BaseMessageHandler));
			
			Assert.That(_subject.HandlersForType<ISubMessage>(), Is.Not.Empty);
		}

		[Test]
		public void handling_a_message_calls_all_matching_message_handlers()
		{

		}

		[Test]
		public void handling_a_messaging_does_not_call_handlers_for_other_types()
		{
		}

		[Test]
		public void handling_a_message_triggers_more_generic_handlers()
		{
		}

		[Test]
		public void handling_a_message_does_not_trigger_more_specific_handlers()
		{
		}

		#region Type junk
		public interface IMessageOne:IMessage { }
		public interface IMessageTwo:IMessage { }
		public interface IMessageThree:IMessage { }

		public interface ISubMessage : IMessageOne { }
		public class SubMessageHandler : IHandle<ISubMessage> { public void Handle(ISubMessage message) { } }

		public class BaseMessageHandler:IHandle<IMessage> { public void Handle(IMessage message) { } }
		public class HandlerOne:IHandle<IMessageOne> { public void Handle(IMessageOne message) { } }
		public class HandlerTwo:IHandle<IMessageTwo> { public void Handle(IMessageTwo message) { } }
		public class HandlerThree:IHandle<IMessageThree> { public void Handle(IMessageThree message) { } }
		#endregion
	}
}