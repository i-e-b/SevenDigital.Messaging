// ReSharper disable InconsistentNaming

using NUnit.Framework;
using SevenDigital.Messaging.Integration.Tests.MessageSending.BaseCases;
using SevenDigital.Messaging.Integration.Tests._Helpers;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class SendingAndReceiving_WithDefaultQueue_Tests:SendingAndReceivingBase
	{
		public override void ConfigureMessaging()
		{
			Helper.SetupTestMessaging();
		}
	}

	
	[TestFixture]
	public class SendingAndReceiving_WithNonPersistentQueue_Tests : SendingAndReceivingBase
	{
		public override void ConfigureMessaging()
		{
			Helper.SetupTestMessagingNonPersistent();
		}
	}
}