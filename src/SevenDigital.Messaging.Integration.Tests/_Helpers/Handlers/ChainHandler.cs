using SevenDigital.Messaging.Integration.Tests._Helpers.Messages;

namespace SevenDigital.Messaging.Integration.Tests._Helpers.Handlers
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
