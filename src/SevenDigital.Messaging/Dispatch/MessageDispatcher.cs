using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatcher : IMessageDispatcher
	{
		readonly IWorkWrapper workWrapper;
		readonly Dictionary<Type, ActionList> handlers;
		int runningHandlers;

		public MessageDispatcher(IWorkWrapper workWrapper)
		{
			this.workWrapper = workWrapper;
			handlers = new Dictionary<Type, ActionList>();
		}

		public void TryDispatch(object messageObject)
		{
			var type = messageObject.GetType().DirectlyImplementedInterfaces().Single();

			var actions = GetMatchingActions(type).SelectMany(t=>t.GetClosed(messageObject)).ToList();

			if (!actions.Any())
			{
				Log.Warning("Ignoring message of type "+type+" because there are no handlers");
				return;
			}

			foreach (var action in actions)
			{
				var handlerWrapper = action;
				workWrapper.Do(() =>
				{
					Interlocked.Increment(ref runningHandlers);
					try
					{
						handlerWrapper();
					} finally
					{
						Interlocked.Decrement(ref runningHandlers);
					}
				});
			}
		}

		IEnumerable<ActionList> GetMatchingActions(Type type)
		{
			return from key in handlers.Keys where key.IsAssignableFrom(type) select handlers[key];
		}

		public void AddHandler<T>(HandlerAction<T> handlerAction) where T : class, IMessage
		{
			lock (handlers)
			{
				if (!handlers.ContainsKey(typeof(T)))
				{
					handlers.Add(typeof(T), new ActionList());
				}
				handlers[typeof(T)].Add(handlerAction);
			}
		}

		public int HandlersInflight { get { return runningHandlers; } }

		public IEnumerable<HandlerAction<T>> HandlersForType<T>() where T : class, IMessage
		{
			return handlers[typeof(T)].GetOfType<T>();
		}

		class ActionList
		{
			readonly List<object> list;

			public ActionList()
			{
				list = new List<object>();
			}

			public void Add<T>(HandlerAction<T> act) where T : class, IMessage
			{
				list.Add(act);
			}

			public IEnumerable<HandlerAction<T>> GetOfType<T>() where T : class, IMessage
			{
				return list.Select(boxed => (HandlerAction<T>)boxed);
			}

			public IEnumerable<Action> GetClosed(object obj)
			{
				return list.Select(boxed => unboxAndEnclose(boxed, obj));
			}

			Action unboxAndEnclose(object boxedAction, object obj)
			{
				var x = (Delegate)boxedAction;
				return () => x.DynamicInvoke(obj);
			}
		}
	}
}