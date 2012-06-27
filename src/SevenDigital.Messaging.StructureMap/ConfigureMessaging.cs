using SevenDigital.Messaging.Domain;
using SevenDigital.Messaging.Services;
using StructureMap;

namespace SevenDigital.Messaging
{
	/// <summary>
	/// Configuration helper for structure map and SevenDigital.Messaging
	/// </summary>
	public class ConfigureMessaging: IHaveDefaults
	{
		/// <summary>
		/// Configure SevenDigital.Messaging with defaults.
		/// After calling this method, you can use the INodeFactory as a collaborator.
		/// The default host is "localhost"
		/// </summary>
		public static IHaveDefaults WithDefaults()
		{
			ObjectFactory.Configure(map => {
				map.For<INodeFactory>().Use<NodeFactory>();
				map.For<IMessagingHost>().Use(()=> new Host("localhost"));
				map.For<ISenderEndpointGenerator>().Use<SenderEndpointGenerator>();
				map.For<IUniqueEndpointGenerator>().Use<UniqueEndpointGenerator>();
				map.For<IServiceBusFactory>().Use<ServiceBusFactory>();
			});
			return new ConfigureMessaging();
		}

		/// <summary>
		/// Configure target messaging host. This should be the IP or hostname of a server 
		/// running RabbitMQ service.
		/// </summary>
		/// <param name="host">IP or hostname of a server running RabbitMQ service</param>
		public IHaveDefaults AndMessagingServer(string host)
		{
			ObjectFactory.Configure(map => map.For<IMessagingHost>().Use(()=> new Host(host)));
			return new ConfigureMessaging();
		}
	}

	public interface IHaveDefaults
	{
		IHaveDefaults AndMessagingServer(string host);
	}
}
