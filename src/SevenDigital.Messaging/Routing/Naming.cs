using System;
using System.Text;
using System.Net.NetworkInformation;
using System.Reflection;

namespace SevenDigital.Messaging.Routing
{
	public static class Naming
	{
		public static string GoodAssemblyName()
		{
			return ( Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
		}

		public static string GetMacAddress()
		{
			const int minMacAddrLength = 12;
			var macAddress = "";
			long maxSpeed = -1;

			foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				var tempMac = nic.GetPhysicalAddress().ToString();
				if (nic.Speed <= maxSpeed || String.IsNullOrEmpty(tempMac) || tempMac.Length < minMacAddrLength) continue;
				maxSpeed = nic.Speed;
				macAddress = tempMac;
			}
			return macAddress;
		}

		public static string MachineName ()
		{
			var sb = new StringBuilder ();
			foreach (char c in Environment.MachineName) {
				if (char.IsLetterOrDigit(c)) sb.Append(c);
			}
			return sb.ToString();
		}
	}
}