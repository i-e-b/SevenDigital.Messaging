using System;
using System.IO;
using System.Net;
using System.Threading;
using NUnit.Framework;
using ServiceStack.Text;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests._Helpers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Api
{
	[TestFixture]
	public class LocalQueueErrorHookTests
	{
		IReceiver _receiver;
		ISenderNode _sender;
		IColourMessage _sampleMessage;
		const string LocalQueuePath = "./localQueue";
		const string ErrorQueuePath = "./errorQueue";
		protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(5); } }

		/*
		 * The plan:
		 * 
		 * Options after Configure.WithLocalQueue()...
		 * allows to set another queue storage location 
		 * which gets written to with all the handler error
		 * messages
		 * 
		 */
		
		[SetUp]
		public void when_a_handler_throws_an_exception ()
		{
			with_local_queue_messaging_and_an_error_queue();
			and_a_triggering_message();

			_receiver.Listen(_=>_.Handle<IColourMessage>().With<ExceptionSample>());
			_sender.SendMessage(_sampleMessage);
		}

		void and_a_triggering_message()
		{
			_sampleMessage = DynamicProxy.GetInstanceFor<IColourMessage>();
			_sampleMessage.CorrelationId = Guid.NewGuid();
			_sampleMessage.Text = "Hello, world";
		}

		void with_local_queue_messaging_and_an_error_queue()
		{
			if (Directory.Exists(LocalQueuePath))
				Directory.Delete(LocalQueuePath, true);

			if (Directory.Exists(ErrorQueuePath))
				Directory.Delete(ErrorQueuePath, true);

			MessagingSystem
				.Configure
				.WithLocalQueue(LocalQueuePath)
				.SendHandlerErrorsToQueue(ErrorQueuePath);

			ObjectFactory.Configure(map => map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());

			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_receiver = MessagingSystem.Receiver();
			_sender = MessagingSystem.Sender();

			ExceptionSample.AutoResetEvent = new AutoResetEvent(false);
			TestTrigger.AutoResetEvent = new AutoResetEvent(false);
		}

		[Test]
		public void handler_errors_should_be_persisted_the_the_designated_queue ()
		{
			Assert.True(ExceptionSample.AutoResetEvent.WaitOne(ShortInterval), "initial message not received");
			MessagingSystem.Control.Shutdown();

			// Hook into the error queue:
			MessagingSystem.Configure.WithLocalQueue(ErrorQueuePath);

			// pick up the (hopefully) waiting message
			_receiver.Listen(_=>_.Handle<IHandlerExceptionMessage>().With<TestTrigger>());
			Assert.True(TestTrigger.AutoResetEvent.WaitOne(ShortInterval), "error message not stored");

			Assert.That(TestTrigger.LastMessage.CausingMessage.CorrelationId , Is.EqualTo(_sampleMessage.CorrelationId));
			Assert.That(TestTrigger.LastMessage.Date.Day, Is.EqualTo(DateTime.UtcNow.Day)); // don't run at midnight :-)
			Assert.That(TestTrigger.LastMessage.Exception, Is.EqualTo("wrong"));
			Assert.That(TestTrigger.LastMessage.HandlerTypeName, Is.EqualTo("wrong again"));
		}

		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }


		[RetryMessage(typeof(IOException))]
		public class ExceptionSample : IHandle<IColourMessage>
		{
			public static int handledTimes = 0;
			readonly object lockobj = new Object();

			public static AutoResetEvent AutoResetEvent { get; set; }

			public void Handle(IColourMessage message)
			{
				try
				{
					lock (lockobj)
					{
						handledTimes++;
					}
					if (handledTimes == 1) throw new IOException();
				}
				finally
				{
					AutoResetEvent.Set();
				}
			}
		}
	}

	public class TestTrigger : IHandle<IHandlerExceptionMessage>
	{
		public static AutoResetEvent AutoResetEvent { get; set; }
		public static IHandlerExceptionMessage LastMessage { get; set; }
		public void Handle(IHandlerExceptionMessage message)
		{
			LastMessage = message;
			AutoResetEvent.Set();
		}
	}
}