using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Logging
{
	[TestFixture]
	public class LoggingTest
	{
		[Test]
		public void Logging_works ()
		{
			new MessagingConfiguration().WithDefaults();
			Log.Warning("This is a warning message");

			var mbase = ObjectFactory.GetInstance<IMessagingBase>();

			var msg = mbase.GetMessage<ILogMessage>("WarningLog");

			Assert.That(msg.Message, Is.EqualTo("This is a warning message"));
		}
	}
}
