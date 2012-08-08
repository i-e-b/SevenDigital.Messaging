using System;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
    public class DummyEventHook:IEventHook
    {
        public void MessageSent(IMessage msg){}
        public void MessageReceived(IMessage msg){}
        public void HandlerFailed(IMessage message, Type handler, Exception ex){}
    }
}