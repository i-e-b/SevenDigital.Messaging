using System;
using System.Threading;
using NUnit.Framework;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class RegisteringMultipleHandlers
	{
		int _messageCount;

		[SetUp]
		public void multiple_waiting_messages()
		{
			Helper.SetupTestMessaging();
			_messageCount = 10;
		}

		[Test]
		public void when_registering_multiple_handlers_messages_should_not_be_lost()
		{
			BHandler.Count = 0;
			Thread.Sleep(500);
			
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.A").Register(
				map => map.Handle<IAMessage>().With<AHandler>(),
				map => {
					for (var i = 0; i < _messageCount; i++)
					{
						MessagingSystem.Sender().SendMessage(new BMessage());
					}
					Thread.Sleep(200);
				},
				map => map.Handle<IBMessage>().With<BHandler>()
			);

			Thread.Sleep(500);
			Assert.That(BHandler.Count, Is.EqualTo(_messageCount));
		}

		[Test]
		public void when_registering_single_handler_for_multiple_message_types_then_messages_should_not_be_lost()
		{
			ABHandler.Count = 0;
			
			MessagingSystem.Receiver().TakeFrom("MultipleHandlers.Integration.Test.B").Register(
				map => map.Handle<IAMessage>().With<ABHandler>(),
				map => {
					for (var i = 0; i < _messageCount; i++)
					{
						MessagingSystem.Sender().SendMessage(new BMessage());
					}
					Thread.Sleep(200);
				},
				map => map.Handle<IBMessage>().With<ABHandler>()
			);

			Thread.Sleep(500);
			Assert.That(ABHandler.Count, Is.EqualTo(_messageCount));
		}

		[TearDown]
		public void teardown()
		{
			MessagingSystem.Control.Shutdown();
		}

		#region Type junk
		public interface IAMessage : IMessage { };
		public interface IBMessage : IMessage { };

		public class AMessage : IAMessage { public Guid CorrelationId { get; set; } }
		public class BMessage : IBMessage { public Guid CorrelationId { get; set; } }

		public class ABHandler : IHandle<IAMessage>, IHandle<IBMessage>
		{

			public void Handle(IAMessage message)
			{Console.WriteLine("ABH,AM");
			}

			public void Handle(IBMessage message)
			{Console.WriteLine("ABH,BM");
				Interlocked.Increment(ref Count);
			}

			public static int Count;
		}
		public class AHandler : IHandle<IAMessage>
		{
			public void Handle(IAMessage message) {
				Console.WriteLine("AH,AM"); } 
		}
		public class BHandler : IHandle<IBMessage>
		{

			public void Handle(IBMessage message)
			{
				Console.WriteLine("BH,BM");
				Interlocked.Increment(ref Count);
			}

			public static int Count;
		}
		#endregion
	}
}
