using System;
using NUnit.Framework;
using StructureMap;

namespace SevenDigital.Messaging.Unit.Tests.LoopbackMessaging
{
	[TestFixture]
	public class LoopbackConfigurationTests
	{
		[SetUp]
		public void When_configuring_with_loopback ()
		{
			DummyHandler.Reset();
			OtherHandler.Reset();
			DifferentHandler.Reset();

			new MessagingConfiguration().WithLoopback();
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_without_waiting ()
		{
			var node = ObjectFactory.GetInstance<INodeFactory>();
			using (var receiver = node.Listener())
			{
				var sender = node.Sender();

				receiver.Handle<IDummyMessage>().With<DummyHandler>();
				sender.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

				Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void Should_receive_messages_at_every_applicable_handler ()
		{
			var node = ObjectFactory.GetInstance<INodeFactory>();
			using (var receiver = node.Listener())
			{
				var sender = node.Sender();

				receiver.Handle<IDummyMessage>().With<DummyHandler>();
				receiver.Handle<IDummyMessage>().With<OtherHandler>();
				receiver.Handle<IDifferentMessage>().With<DifferentHandler>();

				sender.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

				Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
				Assert.That(OtherHandler.CallCount, Is.EqualTo(1));
				Assert.That(DifferentHandler.CallCount, Is.EqualTo(0));
			}
		}

		
		[Test]
		public void Should_receive_competing_messages_at_only_one_handler ()
		{
			var node = ObjectFactory.GetInstance<INodeFactory>();
			var sender = node.Sender();

			var receiver1 = node.ListenOn("Compete");
			var receiver2 = node.ListenOn("Compete");

			receiver1.Handle<IDummyMessage>().With<DummyHandler>();
			receiver2.Handle<IDummyMessage>().With<DummyHandler>();

			sender.SendMessage(new DummyMessage{CorrelationId = Guid.NewGuid()});

			receiver1.Dispose();
			receiver2.Dispose();

			Assert.That(DummyHandler.CallCount, Is.EqualTo(1));
		}
	}

	public class DifferentHandler:IHandle<IDifferentMessage>
	{
		public static int CallCount;
		public void Handle(IDifferentMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public interface IDifferentMessage:IMessage
	{
	}

	public class OtherHandler:IHandle<IDummyMessage>
	{
		public static int CallCount;
		public void Handle(IDummyMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public class DummyMessage:IDummyMessage
	{
		public Guid CorrelationId { get; set; }
	}

	public class DummyHandler:IHandle<IDummyMessage>
	{
		public static int CallCount;
		public void Handle(IDummyMessage message){CallCount++;}
		public static void Reset(){CallCount=0;}
	}

	public interface IDummyMessage:IMessage
	{
	}
}
