using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.Helpers;
using SevenDigital.Messaging.Management;

namespace SevenDigital.Messaging.Integration.Tests
{
	[TestFixture]
	public class RabbitMqRoundTripTests
	{
		[Test]
		public void can_add_list_and_delete_a_queue ()
		{
			var api_endpoint = "http://"+ConfigurationManager.AppSettings["rabbitServer"].SubstringBefore('/')+":55672";
			var vhost = ConfigurationManager.AppSettings["rabbitServer"].SubstringAfterLast('/');
			var api = new RabbitMqApi(api_endpoint, "guest", "guest", vhost);

			var roundTripName = "ApiRoundTrip_"+(Guid.NewGuid().ToString());
			
			api.AddQueue(roundTripName);
			Assert.That(api.ListQueues().Select(q=>q.name), Contains.Item(roundTripName));

			api.DeleteQueue(roundTripName);
			Assert.That(api.ListQueues().Any(q=>q.name == roundTripName), Is.False, "Queue deletion");
		}
	}
}
