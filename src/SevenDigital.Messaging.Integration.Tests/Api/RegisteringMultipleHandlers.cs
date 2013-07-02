using System;
using System.Threading;
using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class RegisteringMultipleHandlers_Base
	{
		protected int _messageCount;

		[SetUp]
		public void multiple_waiting_messages()
		{
			_messageCount = 10;
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
			Helper.SetupTestMessaging();
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.A");
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.B");
			MessagingSystem.Control.Shutdown();
		}

		[Test]
		public void when_registering_multiple_handlers_messages_should_not_be_lost()
		{
			Helper.SetupTestMessagingWithoutPurging();
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.A").Register(
				map => map.Handle<IAMessage>().With<ABHandler>(),
				map => map.Handle<IBMessage>().With<ABHandler>()
			);

			MessagingSystem.Control.Shutdown();
			BHandler.Count = 0;
			Helper.SetupTestMessagingWithoutPurging();

			Thread.Sleep(200);
			var node = MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.A");
			for (var i = 0; i < _messageCount; i++)
			{
				MessagingSystem.Sender().SendMessage(new BMessage());
			}

			node.Register(
				map => map.Handle<IAMessage>().With<AHandler>(),
				map => map.Handle<IBMessage>().With<BHandler>()
				);

			Thread.Sleep(500);
			Assert.That(BHandler.Count, Is.EqualTo(_messageCount));

		}

		[Test]
		public void when_registering_single_handler_for_multiple_message_types_then_messages_should_not_be_lost()
		{
			Helper.SetupTestMessagingWithoutPurging();
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.B").Register(
				map => map.Handle<IAMessage>().With<ABHandler>(),
				map => map.Handle<IBMessage>().With<ABHandler>()
			);
			MessagingSystem.Control.Shutdown();
			
			ABHandler.Count = 0;
			Helper.SetupTestMessagingWithoutPurging();
			
			Thread.Sleep(200);
			for (var i = 0; i < _messageCount; i++)
			{
				MessagingSystem.Sender().SendMessage(new BMessage());
			}

			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.B").Register(
				map => map.Handle<IAMessage>().With<ABHandler>(),
				map => map.Handle<IBMessage>().With<ABHandler>()
			);

			Thread.Sleep(500);
			Console.WriteLine(ABHandler.Count);
			Assert.That(ABHandler.Count, Is.EqualTo(_messageCount));
		}

		#region Type junk
		public interface IAMessage : IMessage { };
		public interface IBMessage : IMessage { };

		public class AMessage : IAMessage { public Guid CorrelationId { get; set; } }
		public class BMessage : IBMessage { public Guid CorrelationId { get; set; } }

		public class ABHandler : IHandle<IAMessage>, IHandle<IBMessage>
		{

			public void Handle(IAMessage message)
			{
				Console.WriteLine("ABH,AM");
			}

			public void Handle(IBMessage message)
			{
				Interlocked.Increment(ref count);
				Console.WriteLine("ABH,BM," + count);
			}

			static volatile int count;

			public static int Count
			{
				get { return count; }
				set { count = value; }
			}
		}
		public class AHandler : IHandle<IAMessage>
		{
			public void Handle(IAMessage message)
			{
				Console.WriteLine("AH,AM");
			}
		}
		public class BHandler : IHandle<IBMessage>
		{

			public void Handle(IBMessage message)
			{
				Interlocked.Increment(ref count);
				Console.WriteLine("BH,BM," + count);
			}

			static volatile int count;

			public static int Count
			{
				get { return count; }
				set { count = value; }
			}
		}
		#endregion
	}

}
