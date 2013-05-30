using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	
	/// <summary>
	/// base implementation for a receiver node in messaging
	/// </summary>
	public class Node : INode
	{
		readonly IMessagingBase messagingBase;
		readonly IDispatchController dispatchController;
		//IDestinationPoller destinationPoller;
		string endpoint;

		/// <summary>
		/// Create a new basic receiver node. This should be created by the ObjectFactory
		/// </summary>
		public Node(IMessagingBase messagingBase, IDispatchController dispatchController)
		{
			this.messagingBase = messagingBase;
			this.dispatchController = dispatchController;
		}

		/// <summary>
		/// Set this node to listen on a messaging endpoint.
		/// </summary>
		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint.ToString();
		}

		/// <summary>
		/// Bind a message type/handler type pair to consume messages on this node
		/// </summary>
		public void SubscribeHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>
		{
			messagingBase.CreateDestination<TMessage>(endpoint);
			dispatchController.AddHandler<TMessage, THandler>(endpoint);
		}

		/// <summary>
		/// Unbind a handler from all messages
		/// </summary>
		public void RemoveHandler<THandler>()
		{
			dispatchController.RemoveHandler<THandler>(endpoint);
		}

		/// <summary>
		/// Stop this node
		/// </summary>
		public void Dispose()
		{
		}

	}
}