using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace SevenDigital.Messaging.Loopback
{
	/// <summary>
	/// Node factory for loopback.
	/// You don't need to create this yourself, use `Messaging.Receiver()` in loopback mode
	/// </summary>
	public class LoopbackReceiver : IReceiver
	{
		readonly Dictionary<Type, ConcurrentBag<Type>> listenerBindings;
		readonly ConcurrentBag<string> capturedEndpoints;

		/// <summary>
		/// Create a loopback factory.
		/// </summary>
		public LoopbackReceiver()
		{
			listenerBindings = new Dictionary<Type, ConcurrentBag<Type>>();
			capturedEndpoints = new ConcurrentBag<string>();
		}
		
		/// <summary>
		/// List all handlers that have been registered on this node.
		/// </summary>
		public IEnumerable<Type> ListenersFor<T>()
		{
			var key = typeof(T);
			return listenerBindings.ContainsKey(key) ? listenerBindings[key].ToList() : new List<Type>();
		}

		/// <summary>
		/// Map handlers to a listener on a named endpoint.
		/// All other listeners on this endpoint will compete for messages
		/// (i.e. only one listener will get a given message)
		/// </summary>
		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			// In the real version, agents compete for incoming messages.
			// In this test version, we only really bind the first listener for a given endpoint -- roughly the same effect!
			if (capturedEndpoints.Contains(endpoint.ToString())) return new DummyReceiver();

			capturedEndpoints.Add(endpoint.ToString());
			return new LoopbackReceiverNode(this);
		}

		/// <summary>
		/// Returns all endpoint names that have been passed using "TakeFrom" rather than "Listen"
		/// </summary>
		public IEnumerable<string> CompetitiveEndpoints()
		{
			return capturedEndpoints;
		}

		/// <summary>
		/// Map handlers to a listener on a unique endpoint.
		/// All listeners mapped this way will receive all messages.
		/// </summary>
		public IReceiverNode Listen()
		{
			return new LoopbackReceiverNode(this);
		}

		/// <summary>
		/// Bind a message to a handler
		/// </summary>
		public void Bind<TMessage, THandler>()
		{
			var msg = typeof(TMessage);
			var handler = typeof(THandler);

			if (!listenerBindings.ContainsKey(msg))
				listenerBindings.Add(msg, new ConcurrentBag<Type>());

			if (listenerBindings[msg].Contains(handler)) return;

			listenerBindings[msg].Add(handler);
		}

		/// <summary>
		/// Send a message to appropriate handlers
		/// </summary>
		public void Send<T>(T message) where T : IMessage
		{
			var hooks = ObjectFactory.GetAllInstances<IEventHook>();
			foreach (var hook in hooks)
			{
				hook.MessageSent(message);
			}

			FireCooperativeListeners(message);
		}

		void FireCooperativeListeners<T>(T message) where T : IMessage
		{
			var msg = message.GetType();
			var matches = listenerBindings.Keys.Where(k => k.IsAssignableFrom(msg));
			foreach (var key in matches)
			{
				var handlers = listenerBindings[key].Select(ObjectFactory.GetInstance);
				foreach (var handler in handlers)
				{
					try
					{
						handler.GetType().InvokeMember("Handle", BindingFlags.InvokeMethod, null, handler, new object[] { message });


						var hooks = ObjectFactory.GetAllInstances<IEventHook>();
						foreach (var hook in hooks)
						{
							hook.MessageReceived(message);
						}
					}
					catch (Exception ex)
					{
						object handler1 = handler;


						var hooks = ObjectFactory.GetAllInstances<IEventHook>();
						foreach (var hook in hooks)
						{
							hook.HandlerFailed(message, handler1.GetType(), ex.InnerException);
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove bindings for the given handler
		/// </summary>
		public void Unregister<T>()
		{
			lock (listenerBindings)
			{
				foreach (var kvp in listenerBindings)
				{
					if (kvp.Value.Any(t => t == typeof(T)))
					{
						var newList = BagOf(kvp.Value, t => t != typeof(T));
						listenerBindings[kvp.Key] = newList;
					}
				}
			}
		}

		static ConcurrentBag<T> BagOf<T>(ConcurrentBag<T> source, Func<T, bool> predicate)
		{
			var b = new ConcurrentBag<T>();
			var s = source.ToArray();
			foreach (var source1 in s.Where(predicate))
			{
				b.Add(source1);
			}
			return b;
		}
	}

}