using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Dispatch;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	public class Node : INode
	{

		readonly IMessagingBase messagingBase;
		readonly IDispatchController dispatchController;
		IDestinationPoller destinationPoller;
		string endpoint;

		public Node(IMessagingBase messagingBase, IDispatchController dispatchController)
		{
			this.messagingBase = messagingBase;
			this.dispatchController = dispatchController;
		}

		public void SetEndpoint(IRoutingEndpoint targetEndpoint)
		{
			endpoint = targetEndpoint.ToString();
			destinationPoller = dispatchController.CreatePoller(endpoint);
		}

		public void SubscribeHandler<TMessage, THandler>()
			where TMessage: class, IMessage 
            where THandler: IHandle<TMessage>
		{
			messagingBase.CreateDestination<TMessage>(endpoint);
			destinationPoller.SetDestinationToWatch(endpoint);
			destinationPoller.AddHandler<TMessage>(typeof(THandler));
			destinationPoller.Start();
		}

		public void Dispose()
		{
			destinationPoller.Stop();
		}

	}
}