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
		public void Logging_works()
		{
			MessagingSystem.Configure.WithDefaults();
			var mbase = ObjectFactory.GetInstance<IMessagingBase>();
			
			Log.Warning("This is a warning message");
			while (mbase.GetMessage<ILogMessage>("WarningLog") != null)
			{
			}

			Log.Warning("This is a warning message");

			var msg = mbase.GetMessage<ILogMessage>("WarningLog");

			Assert.That(msg.Message, Is.EqualTo("This is a warning message"));
			MessagingSystem.Control.Shutdown();
		}
	}
}
