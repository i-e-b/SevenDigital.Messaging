using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
{
	public class ChainHandler : IHandle<IColourMessage>
	{
        readonly ISenderNode _senderNode;

		public ChainHandler(ISenderNode senderNode)
		{
			this._senderNode = senderNode;
		}

		public void Handle(IColourMessage message)
		{
			_senderNode.SendMessage(new JokerMessage());
		}
	}
}
