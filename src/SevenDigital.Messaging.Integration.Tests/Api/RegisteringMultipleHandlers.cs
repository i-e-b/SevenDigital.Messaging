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
			for (var i = 0; i < _messageCount; i++)
			{
				MessagingSystem.Sender().SendMessage(new BMessage());
			}
		}

		[Test]
		public void when_registering_multiple_handlers_messages_should_not_be_lost()
		{
			ABHandler.Count = 0;
			MessagingSystem.Receiver().Listen().Handle<IAMessage>().With<ABHandler>();
			Thread.Sleep(100);
			MessagingSystem.Receiver().Listen().Handle<IBMessage>().With<ABHandler>();

			Thread.Sleep(100);
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

		public class ABHandler:IHandle<IAMessage>, IHandle<IBMessage>
		{

			public void Handle(IAMessage message)
			{
			}

			public void Handle(IBMessage message)
			{
				Interlocked.Increment(ref Count);
			}

			public static int Count;
		}
		#endregion
	}
}
