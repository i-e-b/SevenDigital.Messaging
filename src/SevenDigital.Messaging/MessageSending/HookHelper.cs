using System;
using System.Collections.Generic;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.MessageSending
{
	internal static class HookHelper
	{
		/// <summary>
		/// Fire message sent to registered hooks.
		/// <para>Any exceptions are ignored</para>
		/// </summary>
		public static void TrySentHooks(IMessage message)
		{
			var hooks = GetEventHooks();

			foreach (var hook in hooks)
			{
				try
				{
					hook.MessageSent(message);
				}
				catch (Exception ex)
				{
					Log.Warning("An event hook failed during send " + ex.GetType() + "; " + ex.Message);
				}
			}
		}

		static IEnumerable<IEventHook> GetEventHooks()
		{
			try
			{
				return ObjectFactory.GetAllInstances<IEventHook>();
			}
			catch (Exception ex)
			{
				Log.Warning("Structuremap could not generate event hook list " + ex.GetType() + "; " + ex.Message);
				return new IEventHook[0];
			}
		}
	}
}