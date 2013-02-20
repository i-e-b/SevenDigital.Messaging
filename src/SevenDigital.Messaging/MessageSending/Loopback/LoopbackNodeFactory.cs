using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending.Loopback
{
	public class LoopbackNodeFactory : INodeFactory
	{
		readonly Dictionary<Type, List<Type>> listenerBindings;
		readonly List<string> capturedEndpoints;

		public LoopbackNodeFactory()
		{
			listenerBindings = new Dictionary<Type, List<Type>>();
			capturedEndpoints = new List<string>();
		}

		public List<Type> ListenersFor<T>()
		{
		    var key = typeof (T);
            return listenerBindings.ContainsKey(key) ? listenerBindings[key].ToList() : new List<Type>();
		}

	    public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			// In the real version, agents compete for incoming messages.
			// In this test version, we only really bind the first listener for a given endpoint -- roughly the same effect!
			if (capturedEndpoints.Contains(endpoint.ToString())) return new DummyReceiver();

			capturedEndpoints.Add(endpoint.ToString());
			return new LoopbackReceiver(this);
		}

		/// <summary>
		/// Returns all endpoint names that have been passed using "TakeFrom" rather than "Listen"
		/// </summary>
		public IEnumerable<string> CompetitiveEndpoints()
		{
			return capturedEndpoints;
		}

		public IReceiverNode Listen()
		{
			return new LoopbackReceiver(this);
		}

		public void Bind<TMessage, THandler>()
		{
			var msg = typeof(TMessage);
			var handler = typeof(THandler);

			if (!listenerBindings.ContainsKey(msg))
				listenerBindings.Add(msg, new List<Type>());

			if (listenerBindings[msg].Contains(handler)) return;

			listenerBindings[msg].Add(handler);
		}

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

		public void Unregister<T>()
		{
            lock (listenerBindings)
            {
	            foreach (var kvp in listenerBindings)
	            {
                    if ( kvp.Value.Any(t => t == typeof(T)) )
                    {
	                    var newList = kvp.Value.Where(t => t != typeof (T)).ToList();
	                    listenerBindings[kvp.Key] = newList;
                    }
	            }
            }
		}
	}

}