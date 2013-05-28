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
		readonly IList<IDestinationPoller> pollers;

		/// <summary>
		/// Create a dispatch controller
		/// </summary>
		public DispatchController()
		{
			pollers = new List<IDestinationPoller>();
		}

		/// <summary>
		/// Create a poller for a given destination name
		/// </summary>
		public IDestinationPoller CreatePoller(string destinationName)
		{
			var poller = ObjectFactory.GetInstance<IDestinationPoller>();
			poller.SetDestinationToWatch(destinationName);
			pollers.Add(poller);
			return poller;
		}

		/// <summary>
		/// Stop polling
		/// </summary>
		public void Shutdown()
		{
// ReSharper disable RedundantJumpStatement
			foreach(var poller in pollers.ToArray())
			{
				try { poller.Stop(); } catch { continue; }
			}
// ReSharper restore RedundantJumpStatement
		}
	}
}