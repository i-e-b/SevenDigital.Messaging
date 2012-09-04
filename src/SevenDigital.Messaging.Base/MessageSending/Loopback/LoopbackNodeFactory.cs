using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SevenDigital.Messaging.Routing;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending.Loopback
{
	public class LoopbackNodeFactory:INodeFactory
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
			return listenerBindings[typeof(T)].ToList();
		}

		public IReceiverNode TakeFrom(Endpoint endpoint)
		{
			// In the real version, agents compete for incoming messages.
			// In this test version, we only really bind the first listener for a given endpoint -- roughly the same effect!
			if (capturedEndpoints.Contains(endpoint.ToString())) return new DummyReceiver(); 
			return new LoopbackReceiver(this);
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

			ObjectFactory
				.GetAllInstances<IEventHook>()
				.ForEach(hook => hook.MessageSent(message));

			FireCooperativeListeners(message);
		}

		void FireCooperativeListeners<T>(T message) where T : IMessage
		{
			var msg = typeof(T);
			var matches = listenerBindings.Keys.Where(k => k.IsAssignableFrom(msg));
			foreach (var key in matches)
			{
				var handlers = listenerBindings[key].Select(ObjectFactory.GetInstance);
				foreach (var handler in handlers)
				{
					try
					{
						handler.GetType().InvokeMember("Handle", BindingFlags.InvokeMethod, null, handler, new object[] { message });

						ObjectFactory
							.GetAllInstances<IEventHook>()
							.ForEach(hook => hook.MessageReceived(message));
					}
					catch (Exception ex)
					{
						ObjectFactory
						.GetAllInstances<IEventHook>()
						.ForEach(hook => hook.HandlerFailed(message, handler.GetType(), ex.InnerException));
					}
				}
			}
		}
	}
	
}