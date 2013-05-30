using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace SevenDigital.Messaging.MessageReceiving
{
	/// <summary>
	/// Standard dispatch controller for messaging
	/// </summary>
	public class DispatchController:IDispatchController
	{
		readonly IDictionary<string, IDestinationPoller> pollers;

		/// <summary>
		/// Create a dispatch controller
		/// </summary>
		public DispatchController()
		{
			pollers = new Dictionary<string, IDestinationPoller>();
		}

		public void AddHandler<TMessage, THandler>(string destinationName) where TMessage : class, IMessage where THandler : IHandle<TMessage>
		{
			if (!pollers.ContainsKey(destinationName))
			{
				pollers.Add(destinationName, ObjectFactory.GetInstance<IDestinationPoller>());
				pollers[destinationName].SetDestinationToWatch(destinationName);
			}
			
			var poller = pollers[destinationName];
			poller.AddHandler<TMessage, THandler>();
			poller.Start();
		}

		public void RemoveHandler<T>(string destinationName)
		{
			if (!pollers.ContainsKey(destinationName)) return;

			var poller = pollers[destinationName];
			
			poller.RemoveHandler<T>();
			if (poller.HandlerCount < 1) poller.Stop();
		}

		/// <summary>
		/// Stop polling
		/// </summary>
		public void Shutdown()
		{
// ReSharper disable RedundantJumpStatement
			foreach(var poller in pollers.Values.ToArray())
			{
				try { poller.Stop(); } catch { continue; }
			}
// ReSharper restore RedundantJumpStatement
		}
	}
}