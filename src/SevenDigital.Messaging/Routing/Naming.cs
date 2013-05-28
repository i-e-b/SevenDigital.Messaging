using System;
using System.Text;
using System.Net.NetworkInformation;
using System.Reflection;

namespace SevenDigital.Messaging.Routing
{
	/// <summary>
	/// Utilities for naming endpoints
	/// </summary>
	public static class Naming
	{
		/// <summary>
		/// Best effort name for the current running assembly
		/// </summary>
		public static string GoodAssemblyName()
		{
			return ( Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
		}

		/// <summary>
		/// Best effort at reading the host machine's MAC address
		/// </summary>
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

		/// <summary>
		/// The current host machine name, with only alpha-numeric characters
		/// </summary>
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