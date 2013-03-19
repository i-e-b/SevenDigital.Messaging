using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Message dispatcher keeps a set of bindings 
	/// between message types and handler types.
	/// 
	/// It uses a Work Wrapper to trigger handling of 
	/// incoming messages.
	/// </summary>
	public class MessageDispatcher : IMessageDispatcher
	{
		readonly IWorkWrapper workWrapper;
		readonly Dictionary<Type, HashSet<Type>> handlers; // message type => [handler types]
		int runningHandlers;

		public MessageDispatcher(IWorkWrapper workWrapper)
		{
			this.workWrapper = workWrapper;
			handlers = new Dictionary<Type, HashSet<Type>>();
		}

		public void TryDispatch(IPendingMessage<object> pendingMessage)
		{
			var messageObject = pendingMessage.Message;
			var type = messageObject.GetType().DirectlyImplementedInterfaces().Single();

			var matchingHandlers = GetMatchingHandlers(type).ToList();

			if (!matchingHandlers.Any())
			{
				pendingMessage.Finish();
				Log.Warning("Ignoring message of type " + type + " because there are no handlers");
				return;
			}

			workWrapper.Do(() => HandleMessageWithInstancesOfHandlers(pendingMessage, matchingHandlers, messageObject));
		}

		void HandleMessageWithInstancesOfHandlers(IPendingMessage<object> pendingMessage, IEnumerable<Type> matchingHandlers, object messageObject)
		{
			foreach (var handler in matchingHandlers)
			{
				Interlocked.Increment(ref runningHandlers);
				var hooks = ObjectFactory.GetAllInstances<IEventHook>();

				try
				{
					var instance = ObjectFactory.GetInstance(handler);
					handler.GetMethod("Handle", new[] { messageObject.GetType() }).Invoke(instance, new[] { messageObject });
					FireHandledOkHooks((IMessage) messageObject, hooks);
				}
				catch (Exception ex)
				{
					if (ex is TargetInvocationException)
					{
						ex = ex.InnerException;
					}
					try
					{
						if (ShouldRetry(ex.GetType(), handler)) pendingMessage.Cancel();
						else pendingMessage.Finish();

						FireHandlerFailedHooks((IMessage) messageObject, hooks, ex, handler);
					} catch (Exception exinner)
					{
						Log.Warning("Firing handler failed hooks didn't succeed: " + exinner.Message);
					}
					return;
				}
				finally
				{
					Interlocked.Decrement(ref runningHandlers);
				}
			}
			pendingMessage.Finish();
		}

		static bool ShouldRetry(Type exceptionType, Type handlerType)
		{
			return handlerType.GetCustomAttributes(typeof(RetryMessageAttribute), false)
				.OfType<RetryMessageAttribute>()
				.Any(r => r.RetryExceptionType == exceptionType);
		}


		static void FireHandledOkHooks(IMessage msg, IEnumerable<IEventHook> hooks)
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

		static void FireHandlerFailedHooks(IMessage msg, IEnumerable<IEventHook> hooks, Exception ex, Type handlerType)
		{
			foreach (var hook in hooks)
			{
				try
				{
					hook.HandlerFailed(msg, handlerType, ex);
				}
				catch (Exception exi)
				{
					Console.WriteLine("An event hook failed after handling " + exi.GetType() + "; " + exi.Message);
				}
			}
		}

		IEnumerable<Type> GetMatchingHandlers(Type type)
		{
			return handlers.Keys.Where(k => k.IsAssignableFrom(type)).SelectMany(k => handlers[k]);
		}

		public void AddHandler<TMessage, THandler>()
			where TMessage : class, IMessage
			where THandler : IHandle<TMessage>
		{
			lock (handlers)
			{
				if (!handlers.ContainsKey(typeof(TMessage)))
				{
					handlers.Add(typeof(TMessage), new HashSet<Type> { typeof(THandler) });
				}
				handlers[typeof(TMessage)].Add(typeof(THandler));
			}
		}

		public void RemoveHandler<T>()
		{
			foreach (var hashSet in handlers.Values)
			{
				hashSet.Remove(typeof(T));
			}
		}

		public int CountHandlers()
		{
			return handlers.Values.Sum(hs=>hs.Count);
		}

		public int HandlersInflight { get { return runningHandlers; } }

		public IEnumerable<Type> HandlersForType<T>() where T : class, IMessage
		{
			return handlers[typeof(T)]
				.Select(t => Type.GetType(t.AssemblyQualifiedName ?? ""))
				.ToList();
		}

	}
}