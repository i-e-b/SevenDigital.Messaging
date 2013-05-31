using System;
using System.Collections.Generic;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.MessageReceiving;
using SevenDigital.Messaging.Routing;

namespace SevenDigital.Messaging.MessageSending
{
	/// <summary>
	/// Standard node factory for messaging.
	/// You don't need to create this yourself, use `Messaging.Receiver()`
	/// </summary>
	public class Receiver : IReceiver, IReceiverControl, IDisposable
	{
		readonly IUniqueEndpointGenerator _uniqueEndPointGenerator;
		readonly ISleepWrapper _sleeper;
		readonly IMessagingBase _messageBase;
		readonly IMessageHandler _handler;
		readonly IMessageRouter _messageRouter;
		readonly List<IReceiverNode> _registeredNodes;
		readonly object _lockObject;

		/// <summary>
		/// Create a new node factory.
		/// You don't need to create this yourself, use `Messaging.Receiver()`
		/// </summary>
		public Receiver(
			IUniqueEndpointGenerator uniqueEndPointGenerator,
			ISleepWrapper sleeper,
			IMessagingBase messageBase,
			IMessageHandler handler,
			IMessageRouter messageRouter)
		{
			_uniqueEndPointGenerator = uniqueEndPointGenerator;
			_sleeper = sleeper;
			_messageBase = messageBase;
			_handler = handler;
			_messageRouter = messageRouter;
			_lockObject = new object();
			_registeredNodes = new List<IReceiverNode>();
			PurgeOnConnect = false;
		}

		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			lock (_lockObject)
			{
				if (PurgeOnConnect)
				{
					_messageRouter.Purge(endpoint.ToString());
				}
				var node = new ReceiverNode(this, endpoint, _handler, _messageBase, _sleeper);
				_registeredNodes.Add(node);
				return node;
			}
		}

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		public IReceiverNode Listen()
		{
			return TakeFrom(_uniqueEndPointGenerator.Generate());
		}

		/// <summary>
		/// Close all receiver nodes that have been created
		/// </summary>
		public void Shutdown()
		{
			lock (_lockObject)
			{
				foreach (var node in _registeredNodes)
				{
					node.Dispose();
				}
				_registeredNodes.Clear();
			}
		}

		/// <summary>
		/// Unregister a node from the shutdown list
		/// </summary>
		public void Remove(IReceiverNode node)
		{
			lock (_lockObject)
			{
				if (_registeredNodes.Contains(node))
					_registeredNodes.RemoveAll(n=> Equals(n, node));
			}
		}

		/// <summary>
		/// Set maximum concurrent handlers per receiver node
		/// </summary>
		public void SetConcurrentHandlers(int max)
		{
			lock (_lockObject)
			{
				foreach (var node in _registeredNodes)
				{
					node.SetConcurrentHandlers(max);
				}
			}
		}

		public bool PurgeOnConnect { get; set; }

		/// <summary>
		/// Shutdown all nodes.
		/// </summary>
		public void Dispose()
		{
			Shutdown();
		}
	}
}