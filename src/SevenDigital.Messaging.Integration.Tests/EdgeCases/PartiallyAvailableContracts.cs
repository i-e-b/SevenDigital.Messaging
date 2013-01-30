using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Integration.Tests.EdgeCases
{
	[TestFixture]
	public class PartiallyAvailableContracts
	{
		INodeFactory _nodeFactory;

		[SetUp]
        public void SetUp()
        {
			Helper.SetupTestMessagingWithoutPurging();
			new MessagingConfiguration().ClearEventHooks();
            _nodeFactory = ObjectFactory.GetInstance<INodeFactory>();
        }

		[Test]
		public void when_a_message_has_a_more_specific_type_than_we_support_should_use_most_specific_available ()
		{
			var cid = Guid.Parse("05c90feb5c1041799fc0d26dda5fd1c6");

			using (var listener = _nodeFactory.TakeFrom("TestListener_edgecases"))
			{

				// Simulate sending a message with an unavailable type
				SimulateUnknownMessage(cid);

				listener.Handle<ISpecificMessage>().With<SampleHandler>();

				var result = SampleHandler.Trigger.WaitOne(TimeSpan.FromSeconds(2));
				Assert.That(result, Is.True, "Did not pick up message");
				Assert.That(SampleHandler.LastMessage.CorrelationId, Is.EqualTo(cid));
				Assert.That(SampleHandler.LastMessage.Message, Is.EqualTo("hello"));
			}
		}

		[Test]
		public void can_still_receive_messages_without_contract_stack ()
		{
			var cid = Guid.Parse("05c90feb5c1041799fc0d26dda5fd1c6");

			using (var listener = _nodeFactory.TakeFrom("TestListener_edgecases"))
			{

				// Simulate sending a message with an unavailable type
				SimulateOldStyleMessage(cid);

				listener.Handle<ISpecificMessage>().With<SampleHandler>();

				var result = SampleHandler.Trigger.WaitOne(TimeSpan.FromSeconds(2));
				Assert.That(result, Is.True, "Did not pick up message");
				Assert.That(SampleHandler.LastMessage.CorrelationId, Is.EqualTo(cid));
				Assert.That(SampleHandler.LastMessage.Message, Is.EqualTo("hello"));
			}
		}

		static void SimulateUnknownMessage(Guid cid)
		{
			var router = ObjectFactory.GetInstance<IMessageRouter>();

			var wrong = "Not.A.Real.Type, Example.Types";
// ReSharper disable PossibleNullReferenceException
			var correct = string.Join(", ", typeof(ISpecificMessage).AssemblyQualifiedName.Split(',').Take(2));
			var imsg = string.Join(", ", typeof(IMessage).AssemblyQualifiedName.Split(',').Take(2));
// ReSharper restore PossibleNullReferenceException

			var sample = "{\"__type\":\"" + wrong + "\",\"Message\":\"hello\",\"CorrelationId\":\"05c90feb5c1041799fc0d26dda5fd1c6\",\"HashValue\":123124512," +
		                "\"__contracts\":\"" +
		                wrong + ";" +
						correct + ";" +
						imsg + "\"}";
			
			router.AddSource("TestExchange_edgecases");
			router.AddDestination("TestListener_edgecases");
			router.Link("TestExchange_edgecases", "TestListener_edgecases");
			router.Send("TestExchange_edgecases", sample);
		}

		static void SimulateOldStyleMessage(Guid cid)
		{
			var router = ObjectFactory.GetInstance<IMessageRouter>();

// ReSharper disable PossibleNullReferenceException
			var correct = string.Join(", ", typeof(ISpecificMessage).AssemblyQualifiedName.Split(',').Take(2));
// ReSharper restore PossibleNullReferenceException

			var sample = "{\"__type\":\"" + correct + "\",\"Message\":\"hello\",\"CorrelationId\":\"05c90feb5c1041799fc0d26dda5fd1c6\",\"HashValue\":123124512}";
			
			router.AddSource("TestExchange_edgecases");
			router.AddDestination("TestListener_edgecases");
			router.Link("TestExchange_edgecases", "TestListener_edgecases");
			router.Send("TestExchange_edgecases", sample);
		}

		public class SampleHandler:IHandle<ISpecificMessage>
		{
			public static AutoResetEvent Trigger = new AutoResetEvent(false);
			public static ISpecificMessage LastMessage;

			public void Handle(ISpecificMessage message)
			{
				LastMessage = message;
				Trigger.Set();
			}
		}

	}
		public interface ISpecificMessage:IMessage
		{
			string Message { get; set; }
		}
}
