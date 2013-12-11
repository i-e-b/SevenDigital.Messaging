using System;
using System.IO;
using System.Net;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests._Helpers;
using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Api
{
	[TestFixture]
	public class LocalQueueErrorHookTests
	{
		IReceiver _receiver;
		ISenderNode _senderNode;
		const string LocalQueuePath = "./localQueue";

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
		public void SetUp()
		{
			if (Directory.Exists(LocalQueuePath))
				Directory.Delete(LocalQueuePath, true);

			MessagingSystem.Configure.WithLocalQueue(LocalQueuePath);

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());

			MessagingSystem.Events.AddEventHook<ConsoleEventHook>();
			_receiver = MessagingSystem.Receiver();
			_senderNode = MessagingSystem.Sender();
		}

		[Test]
		public void stub()
		{
			Assert.Inconclusive();
		}

		
		[TestFixtureTearDown]
		public void Stop() { MessagingSystem.Control.Shutdown(); }


		[RetryMessage(typeof(IOException))]
		[RetryMessage(typeof(WebException))]
		public class ExceptionSample : IHandle<IColourMessage>
		{
			public static int handledTimes = 0;
			readonly object lockobj = new Object();

			public static AutoResetEvent AutoResetEvent { get; set; }

			public void Handle(IColourMessage message)
			{
				lock (lockobj)
				{
					handledTimes++;
				}

				if (handledTimes == 1)
				{
					throw new IOException();
				}
				AutoResetEvent.Set();
				throw new InvalidOperationException();
			}
		}
	}
}