using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace SevenDigital.Messaging.Dispatch
{
	public class DispatchController:IDispatchController
	{
		readonly IList<IDestinationPoller> pollers;

		public DispatchController()
		{
			pollers = new List<IDestinationPoller>();
		}

		public IDestinationPoller CreatePoller(string destinationName)
		{
			var poller = ObjectFactory.GetInstance<IDestinationPoller>();
			poller.SetDestinationToWatch(destinationName);
			pollers.Add(poller);
			return poller;
		}

		public void Shutdown()
		{
// ReSharper disable RedundantJumpStatement
			foreach(var poller in pollers.ToArray())
			{
				try { poller.Stop(); } catch { continue; }
			}
// ReSharper restore RedundantJumpStatement
		}

		public void Dispose()
		{
			Shutdown();
		}
	}
}