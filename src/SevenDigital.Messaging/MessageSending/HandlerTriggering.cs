namespace SevenDigital.Messaging.MessageSending
{
	public class HandlerTriggering<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly INode listenerNode;

		public HandlerTriggering(INode listenerNode)
		{
			this.listenerNode = listenerNode;
		}

		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			listenerNode.SubscribeHandler<TMessage, THandler>();
		}
	}
}
