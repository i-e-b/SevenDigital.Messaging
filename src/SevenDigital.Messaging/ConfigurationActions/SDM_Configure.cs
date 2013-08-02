using System;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.Loopback;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.MessageReceiving.RabbitPolling;
using SevenDigital.Messaging.MessageSending;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.ConfigurationActions
{
	class SDM_Configure : IMessagingConfigure
	{
		public IMessagingConfigureOptions WithDefaults()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.IsConfigured() || MessagingSystem.UsingLoopbackMode())
					return new SDM_ConfigureOptions();

				SDM_Control.EjectAndDispose<IReceiverControl>();
				SDM_Control.EjectAndDispose<IReceiver>();

				new MessagingBaseConfiguration().WithDefaults();
				Cooldown.Activate();
				AutoShutdown.Activate();

				ObjectFactory.Configure(map => {
					// Base messaging
					map.For<IMessagingHost>().Use(() => new Host("localhost"));
					map.For<IRabbitMqConnection>().Use(() => new RabbitMqConnection("localhost"));
					map.For<IUniqueEndpointGenerator>().Singleton().Use<UniqueEndpointGenerator>();

					// threading and dispatch
					map.For<IHandlerManager>().Use<HandlerManager>();
					map.For<IPollingNodeFactory>().Use<RabbitMqPollingNodeFactory>();
					map.For<IDispatcherFactory>().Use<DispatcherFactory>();

					// singletons
					map.For<ISleepWrapper>().Singleton().Use<SleepWrapper>();
					map.For<IReceiver>().Singleton().Use<Receiver>();
					map.For<ISenderNode>().Singleton().Use<SenderNode>();
					map.For<IOutgoingQueueFactory>().Singleton().Use<PersistentQueueFactory>();

					// aliases
					map.For<IReceiverControl>().Use(() => ObjectFactory.GetInstance<IReceiver>() as IReceiverControl);
				});

				return new SDM_ConfigureOptions();
			}
		}

		public void WithLoopbackMode()
		{
			lock (MessagingSystem.ConfigurationLock)
			{
				if (MessagingSystem.UsingLoopbackMode()) return;
				if (MessagingSystem.IsConfigured())
					throw new InvalidOperationException("Messaging system has already been configured. You should set up loopback mode first.");

				new MessagingBaseConfiguration().WithDefaults();
				ObjectFactory.EjectAllInstancesOf<IReceiver>();

				ObjectFactory.Configure(map => {
					map.For<ILoopbackBinding>().Singleton().Use<LoopbackBinding>();
					map.For<IReceiver>().Singleton().Use<LoopbackReceiver>();
					map.For<ISenderNode>().Singleton().Use<LoopbackSender>();
					map.For<ITestEvents>().Singleton().Use<TestEvents>();
				});

				MessagingSystem.Events.AddEventHook<TestEventHook>();
			}
		}
	}
}