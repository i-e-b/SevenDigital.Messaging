using System;
using System.Collections.Generic;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	public class HandlerTriggering<TMessage> : IMessageBinding<TMessage> where TMessage : class, IMessage
	{
		readonly INode listenerNode;

		public HandlerTriggering(INode listenerNode)
		{
			this.listenerNode = listenerNode;
		}

		public void With<THandler>() where THandler : IHandle<TMessage>
		{
			listenerNode.SubscribeHandler<TMessage>(msg =>
			{

				var hooks = ObjectFactory.GetAllInstances<IEventHook>();

				try
				{
					ObjectFactory.GetInstance<THandler>().Handle(msg);

				}
				catch (Exception ex)
				{
					FireHandlerFailedHooks<THandler>(msg, hooks, ex);
					return;
				}
				FireHandledOkHooks(msg, hooks);

			});
		}

		static void FireHandledOkHooks(TMessage msg, IEnumerable<IEventHook> hooks)
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

		static void FireHandlerFailedHooks<THandler>(TMessage msg, IEnumerable<IEventHook> hooks, Exception ex) where THandler : IHandle<TMessage>
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
	}
}
