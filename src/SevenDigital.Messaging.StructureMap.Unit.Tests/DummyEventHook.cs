using System;

namespace SevenDigital.Messaging.StructureMap.Unit.Tests
{
    public class DummyEventHook:IEventHook
    {
        public void MessageSent(IMessage msg, string x, string y){}
        public void MessageReceived(IMessage msg, string x){}
        public void HandlerFailed(IMessage message, Type handler, Exception ex){}
    }
}