using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using SevenDigital.Messaging.MessageSending;
using StructureMap;

namespace SevenDigital.Messaging.Dispatch
{
	public class MessageDispatcher : IMessageDispatcher
	{
		readonly IWorkWrapper workWrapper;
		readonly Dictionary<Type, List<Type>> handlers; // message type => [handler types]
		int runningHandlers;

		public MessageDispatcher(IWorkWrapper workWrapper)
		{
			this.workWrapper = workWrapper;
			handlers = new Dictionary<Type, List<Type>>();
		}

		public void TryDispatch(IPendingMessage<object> pendingMessage)
		{
            var messageObject = pendingMessage.Message;
			var type = messageObject.GetType().DirectlyImplementedInterfaces().Single();

			var matchingHandlers = GetMatchingHandlers(type).ToList();

			if (!matchingHandlers.Any())
			{
				pendingMessage.Finish();
				Log.Warning("Ignoring message of type "+type+" because there are no handlers");
				return;
			}

			pendingMessage.Finish();// temp until finished retry logic

			workWrapper.Do(() =>
			{
				foreach (var handler in matchingHandlers)
				{
					Interlocked.Increment(ref runningHandlers);
					try
					{
						var instance = ObjectFactory.GetInstance(handler);
						handler.GetMethod("Handle").MakeGenericMethod(type).Invoke(instance, new object[] { messageObject } );
						//(instance as IHandle<IMessage>).Handle(messageObject);
						//handlerWrapper();
					}
					/*catch (Exception ex)
					{

					}*/
					finally
					{
						Interlocked.Decrement(ref runningHandlers);
					}
				}
			});
		}


		static void FireHandledOkHooks<TMessage>(TMessage msg, IEnumerable<IEventHook> hooks) where TMessage : IMessage
		{
			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageReceived(msg);
				}
				catch (Exception ex)
				{
					Console.WriteLine("An event hook failed after handling " + ex.GetType() + "; " + ex.Message);
				}
			}
		}

		static void FireHandlerFailedHooks<TMessage, THandler>(TMessage msg, IEnumerable<IEventHook> hooks, Exception ex)
			where TMessage : IMessage
			where THandler : IHandle<TMessage>
		{
			foreach (var hook in hooks)
			{
				try
				{
					hook.HandlerFailed(msg, typeof(THandler), ex);
				}
				catch (Exception exi)
				{
					Console.WriteLine("An event hook failed after handling " + exi.GetType() + "; " + exi.Message);
				}
			}
		}
		

		IEnumerable<Type> GetMatchingHandlers(Type type)
		{
            return handlers.Keys.Where(k=>k.IsAssignableFrom(type)).SelectMany(k => handlers[k]);
            //return from key in handlers.Keys where key.IsAssignableFrom(type) select handlers[key];
		}

		public void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
            where THandler : IHandle<TMessage>
		{
			lock (handlers)
			{
				if (!handlers.ContainsKey(typeof (TMessage)))
				{
					handlers.Add(typeof(TMessage), new List<Type> { typeof(THandler) });
				}
				handlers[typeof(TMessage)].Add(typeof(THandler));
			}
		}

		public int HandlersInflight { get { return runningHandlers; } }

		public IEnumerable<IHandle<T>> HandlersForType<T>() where T : class, IMessage
		{
			return handlers[typeof(T)].Cast<IHandle<T>>();
		}

	}
}