using System;
using System.Threading;
using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.MessageSending
{
	[TestFixture]
	public class StressTests
	{
		INodeFactory nodeFactory;
		ISenderNode sender;

		[SetUp]
        public void SetUp()
        {
			Helper.SetupTestMessaging();
			new MessagingConfiguration().ClearEventHooks();
            nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
			sender = ObjectFactory.GetInstance<ISenderNode>();
        }

		[TearDown]
		public void Teardown()
		{
			Console.WriteLine("Cleaning queues");
			Helper.RemoveAllRoutingFromThisSession();
		}

		[Test]
		public void should_be_able_to_send_and_receive_1000_messages_per_minute ()
		{
			using (var listener = nodeFactory.TakeFrom("ping-pong-endpoint"))
			{
				listener.Handle<IPing>().With<PingHandler>();
				listener.Handle<IPong>().With<PongHandler>();

				sender.SendMessage(new PingMessage());

				var result = PongHandler.Trigger.WaitOne(TimeSpan.FromSeconds(60));
				Assert.True(result);
			}
		}
	}

	public class PongHandler : IHandle<IPong>
	{
		public static AutoResetEvent Trigger = new AutoResetEvent(false);
		readonly ISenderNode sender;
		public static int Count = 0;

		public PongHandler(ISenderNode sender)
		{
			this.sender = sender;
		}

		public void Handle(IPong message)
		{
			Interlocked.Increment(ref Count);

			if (Count > 500)
			{
				Console.WriteLine("OK!");
				Trigger.Set();
			}
			else
			{
				sender.SendMessage(new PingMessage());
			}
		}
	}

	public class PingMessage:IPing
	{
		public PingMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
	}

	public class PingHandler : IHandle<IPing>
	{
		readonly ISenderNode sender;

		public PingHandler(ISenderNode sender)
		{
			this.sender = sender;
		}

		public void Handle(IPing message)
		{
			sender.SendMessage(new PongMessage());
		}
	}

	public class PongMessage:IPong
	{
		public PongMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
	}

	public interface IPong:IMessage
	{
	}

	public interface IPing:IMessage
	{
	}
}
