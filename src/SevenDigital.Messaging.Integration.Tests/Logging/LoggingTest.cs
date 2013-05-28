using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Logging;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.Logging
{
	[TestFixture]
	public class LoggingTest
	{
		[Test, Explicit]
		public void Logging_works()
		{
			Messaging.Configure.WithDefaults();
			var mbase = ObjectFactory.GetInstance<IMessagingBase>();

			while (mbase.GetMessage<ILogMessage>("WarningLog") != null)
			{
			}

			Log.Warning("This is a warning message");

			var msg = mbase.GetMessage<ILogMessage>("WarningLog");

			Assert.That(msg.Message, Is.EqualTo("This is a warning message"));
		}
	}
}
