using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Infrastructure;
using SevenDigital.Messaging.MessageSending;

namespace SevenDigital.Messaging.Unit.Tests.MessageSending
{
	[TestFixture]
	public class SenderNodeTests
	{
		ISenderNode _subject;
		IMessagingBase _messagingBase;
		IDispatcherFactory _dispatcherFactory;

		[SetUp]
		public void setup()
		{
			_messagingBase = Substitute.For<IMessagingBase>();
			_dispatcherFactory = Substitute.For<IDispatcherFactory>();

			_subject = new SenderNode(_messagingBase, _dispatcherFactory);
		}

		[Test]
		public void _todo()
		{
			_subject.Dispose();
			Assert.Inconclusive();
		}

	}
}