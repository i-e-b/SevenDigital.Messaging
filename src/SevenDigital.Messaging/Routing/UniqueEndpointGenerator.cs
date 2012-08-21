using System;
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
			var bytes = MD5.Create().ComputeHash(
					Encoding.Default.GetBytes(Assembly.GetExecutingAssembly().CodeBase 
					+ Naming.GetMacAddress())
				);
			strongName =  
				Naming.MachineName()
				+ "_" 
				+ Naming.GoodAssemblyName() 
				+ "_" 
				+ Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "");
		
		}

		public Endpoint Generate()
		{
			return new Endpoint(strongName);
		}
	}
}