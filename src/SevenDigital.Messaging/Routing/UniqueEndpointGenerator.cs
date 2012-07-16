using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SevenDigital.Messaging.Routing
{
	public class UniqueEndpointGenerator : IUniqueEndpointGenerator
	{
		readonly string strongName;

		public UniqueEndpointGenerator()
		{
			strongName = GetStrongName();
		}

		public Endpoint Generate()
		{
			return new Endpoint(strongName);
		}

		static string GetStrongName()
		{
			var bytes = MD5.Create().ComputeHash(Encoding.Default.GetBytes(Assembly.GetExecutingAssembly().CodeBase + GetMacAddress()));
			return 
				Environment.MachineName 
				+ "_" 
				+ GoodAssemblyName() 
				+ "_" 
				+ Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "");
		}

		static string GoodAssemblyName()
		{
			return ( Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;
		}

		static string GetMacAddress()
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
	}
}