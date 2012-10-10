using System;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class SenderNode : ISenderNode
	{
		readonly Node node;

		public SenderNode(IMessagingHost host, ISenderEndpointGenerator endpointGenerator, IServiceBusFactory serviceBusFactory)
		{
			var endpoint = endpointGenerator.Generate();
			node = new Node(host, endpoint, serviceBusFactory);
		}

		public virtual void SendMessage<T>(T message) where T : class, IMessage
		{
			var hooks = ObjectFactory.GetAllInstances<IEventHook>();

			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageSent(message);
				}
				catch (Exception ex)
				{
					Console.WriteLine("An event hook failed during send: " + ex.GetType() + "; " + ex.Message);
				}
			}
			node.EnsureConnection().Publish(message, c => { });
		}

		public bool Equals(SenderNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.node, node);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(SenderNode)) return false;
			return Equals((SenderNode)obj);
		}

		public override int GetHashCode()
		{
			return (node != null ? node.GetHashCode() : 0);
		}
	}
}