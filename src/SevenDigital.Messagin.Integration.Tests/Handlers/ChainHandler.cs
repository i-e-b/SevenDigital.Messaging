using SevenDigital.Messaging.Integration.Tests.Messages;

namespace SevenDigital.Messaging.Integration.Tests.Handlers
{
	public class ChainHandler : IHandle<IColourMessage>
	{
		readonly INodeFactory factory;

		public ChainHandler(INodeFactory factory)
		{
			this.factory = factory;
		}

		public void Handle(IColourMessage message)
		{
			factory.Sender().SendMessage(new JokerMessage());
		}
	}
}
