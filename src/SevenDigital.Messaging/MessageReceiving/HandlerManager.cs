using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
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
	public class HandlerManager : IHandlerManager
	{
		readonly Dictionary<Type, ISet<Type>> _handlers; // message type => [handler types]

		/// <summary>
		/// New dispatcher
		/// </summary>
		public HandlerManager()
		{
			_handlers = new Dictionary<Type, ISet<Type>>();
		}

		/// <summary>
		/// Try to fire actions for a message
		/// </summary>
		public void TryHandle(IPendingMessage<object> pendingMessage)
		{
			var messageObject = pendingMessage.Message;
			var type = TypeExtensions.DirectlyImplementedInterfaces(messageObject.GetType()).Single();

			var matchingHandlers = GetMatchingHandlers(type).ToList();

			if (!matchingHandlers.Any())
			{
				pendingMessage.Finish();
				Log.Warning("Ignoring message of type " + type + " because there are no handlers");
				return;
			}

			HandleMessageWithInstancesOfHandlers(pendingMessage, matchingHandlers, messageObject);
		}

		static void HandleMessageWithInstancesOfHandlers(IPendingMessage<object> pendingMessage, IEnumerable<Type> matchingHandlers, object messageObject)
		{
			foreach (var handler in matchingHandlers)
			{
				var hooks = ObjectFactory.GetAllInstances<IEventHook>();

				try
				{
					var instance = ObjectFactory.GetInstance(handler);
					handler.GetMethod("Handle", new[] { messageObject.GetType() }).Invoke(instance, new[] { messageObject });
					FireHandledOkHooks((IMessage)messageObject, hooks);
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

						FireHandlerFailedHooks((IMessage)messageObject, hooks, ex, handler);
					}
					catch (Exception exinner)
					{
						Log.Warning("Firing handler failed hooks didn't succeed: " + exinner.Message);
					}
					return;
				}
			}
			pendingMessage.Finish();
		}

		/// <summary>
		/// Determines if a Handler is marked for retry of the given exception type
		/// </summary>
		public static bool ShouldRetry(Type exceptionType, Type handlerType)
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
					Log.Warning("An event hook failed after handling " + ex.GetType() + "; " + ex.Message);
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
					Log.Warning("An event hook failed after handling " + exi.GetType() + "; " + exi.Message);
				}
			}
		}

		/// <summary>
		/// List handlers that could process a given message type.
		/// More generic handlers will be returned for more specific message types
		/// </summary>
		public IEnumerable<Type> GetMatchingHandlers(Type type)
		{
			if (_handlers.Count < 1) return new Type[0];
			return _handlers.Keys.Where(k => k.IsAssignableFrom(type)).SelectMany(k => _handlers[k]);
		}

		/// <summary>
		/// Add a handler/message binding
		/// </summary>
		public void AddHandler(Type messageType, Type handlerType)
		{
			lock (_handlers)
			{
				if (!_handlers.ContainsKey(messageType))
				{
					_handlers.Add(messageType, new ConcurrentSet<Type> { handlerType });
				}
				_handlers[messageType].Add(handlerType);
			}
		}

		/// <summary>
		/// remove a handler for all messages
		/// </summary>
		public void RemoveHandler(Type handlerType)
		{
			foreach (var set in _handlers.Values)
			{
				set.Remove(handlerType);
			}
		}

		/// <summary>
		/// Return count of handlers
		/// </summary>
		public int CountHandlers()
		{
			return _handlers.Values.Sum(hs=>hs.Count);
		}

	}
}