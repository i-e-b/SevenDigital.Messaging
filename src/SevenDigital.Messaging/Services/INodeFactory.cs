using SevenDigital.Messaging.Domain;

namespace SevenDigital.Messaging.Services
{
	public interface INodeFactory
	{
		IReceiverNode ListenOn(Endpoint endpoint);
		IReceiverNode Listener();
		ISenderNode SendOn(Endpoint endpoint);
		ISenderNode Sender();
	}
}