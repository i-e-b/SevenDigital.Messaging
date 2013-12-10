// ReSharper disable InconsistentNaming

using System.Configuration;
using System.IO;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.MessageSending.BaseCases;
using SevenDigital.Messaging.Integration.Tests._Helpers;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	/*
	 * What is this?
	 * =============
	 * 
	 * A suite of tests covering basic send/receive behaviour
	 * for all the various modes messaging can be started in:
	 *  - Default (RabbitMQ, Store-and-Forward), uses purging
	 *  - No Persist (RabbitMQ directly), uses purging
	 *  - Loopback mode (no RabbitMQ, no threading, no real queueing)
	 *  - Local queue (no RabbitMQ, Store-and-Forward, with real queueing and multi-process support)
	 *
	 */

	[TestFixture]
	public class SendingAndReceiving_WithDefaultQueue_Tests:SendingAndReceivingBase
	{
		public override void ConfigureMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			MessagingSystem.Configure.WithDefaults().SetMessagingServer(server).SetIntegrationTestMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}

		public override int ExpectedCompeteMessages(int handlers, int sent) { return sent; }
	}
	
	[TestFixture]
	public class SendingAndReceiving_WithNonPersistentQueue_Tests : SendingAndReceivingBase
	{
		public override void ConfigureMessaging()
		{
			var server = ConfigurationManager.AppSettings["rabbitServer"];
			MessagingSystem.Configure.WithDefaults().NoPersistentMessages()
				.SetMessagingServer(server).SetIntegrationTestMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}

		public override int ExpectedCompeteMessages(int handlers, int sent) { return sent; }
	}
	
	[TestFixture]
	public class SendingAndReceiving_WithLoopbackMode_Tests : SendingAndReceivingBase
	{
		public override void ConfigureMessaging()
		{
			MessagingSystem.Configure.WithLoopbackMode();

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}

		// TODO: look into this, it may actually be a bug.
		public override int ExpectedCompeteMessages(int handlers, int sent) { return sent * handlers; }
	}
	
	[TestFixture]
	public class SendingAndReceiving_WithLocalPersistentQueue_Tests : SendingAndReceivingBase
	{
		const string LocalQueuePath = "./localQueue";
		public override void ConfigureMessaging()
		{
			if (Directory.Exists(LocalQueuePath))
				Directory.Delete(LocalQueuePath, true);

			MessagingSystem.Configure.WithLocalQueue(LocalQueuePath);

			ObjectFactory.Configure(map=>map.For<IUniqueEndpointGenerator>().Use<TestEndpointGenerator>());
		}
		
		public override int ExpectedCompeteMessages(int handlers, int sent) { return sent; }
	}
}