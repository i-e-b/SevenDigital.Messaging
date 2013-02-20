using System;// ReSharper disable InconsistentNaming
using System.IO;
using System.Net;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.EventHooks;
using SevenDigital.Messaging.Integration.Tests.Messages;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests
{
    [TestFixture]
    public class RetryReceivingTests
    {
        INodeFactory _nodeFactory;
        private ISenderNode _senderNode;

        protected TimeSpan LongInterval { get { return TimeSpan.FromSeconds(20); } }
        protected TimeSpan ShortInterval { get { return TimeSpan.FromSeconds(3); } }

        [SetUp]
        public void SetUp()
        {
			Helper.SetupTestMessaging();
            ObjectFactory.Configure(map => map.For<IEventHook>().Use<ConsoleEventHook>());
            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
            _senderNode = ObjectFactory.GetInstance<ISenderNode>();
        }

	    [Test]
        public void Handler_should_retry_on_matched_exceptions_but_not_un_unmatched_exceptions ()
        {
            ExceptionSample.AutoResetEvent = new AutoResetEvent(false);
            using (var receiverNode = _nodeFactory.Listen())
            {
                receiverNode.Handle<IColourMessage>().With<ExceptionSample>();

                _senderNode.SendMessage(new RedMessage());

                ExceptionSample.AutoResetEvent.WaitOne(ShortInterval);
                Assert.That(ExceptionSample.handledTimes, Is.EqualTo(2));
            }
        }

		[TestFixtureTearDown]
		public void Stop() { new MessagingConfiguration().Shutdown(); }


		[RetryMessage(typeof(IOException))]
		[RetryMessage(typeof(WebException))]
		public class ExceptionSample : IHandle<IColourMessage>
		{
            public static int handledTimes = 0;
			readonly object lockobj = new Object();

			public static AutoResetEvent AutoResetEvent { get; set; }

			public void Handle(IColourMessage message)
			{
                lock (lockobj)
                {
                    handledTimes++;
                }

				if (handledTimes == 1)
				{
					throw new IOException();
				}
				AutoResetEvent.Set();
				throw new InvalidOperationException();
			}
		}
    }
}