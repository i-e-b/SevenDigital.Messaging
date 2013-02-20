namespace SevenDigital.Messaging.Dispatch
{
	public interface IDestinationPoller
	{
		void SetDestinationToWatch(string targetDestination);
		void Start();
		void Stop();
		void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
            where THandler : IHandle<TMessage>;

		void RemoveHandler<T>();
		int HandlerCount { get; }
	}
}