SevenDigital.Messaging
======================
A distributed contracts-based sender/handler messaging system built on RabbitMQ and BearBones-Messaging

Installation
------------
Windows

* Install [Erlang](http://www.erlang.org/download.html) and [RabbitMQ server](http://www.rabbitmq.com/download.html)

Linux

* As Windows, or use your distro's package manager to get RabbitMQ (you should get Erlang automatically)

Getting Started: Path of least resistance
-----------------------------------------
* Add NuGet reference from [SevenDigital.Messaging](https://nuget.org/packages/SevenDigital.Messaging)

Configure messaging:
```csharp
Messaging.Configure.WithDefaults().WithMessagingServer("localhost");
```

Define a message:
```csharp
// Message contract (defined in both sender and receiver)
public interface IExampleMessage : IMessage {
	string Hello {get;set;}
}

// A specific instance (only needs to be defined in sender)
class MyExample : IExampleMessage {
	public string Hello {get;set;}
}
```

Setup a message handler:
```csharp
public class MyHandler : IHandle<IExampleMessage> {
	public void Handle(IExampleMessage msg) {
		Console.WriteLine("Hello, "+msg.Hello);
	}
}
```

Register handler with listener:
```csharp
// Spawns sets of background threads to handle incoming messages
using (var node = Messaging.Receiver().Listen()) {
	node.Handle<IExampleMessage>().With<MyHandler>();
	
	while (true) {Thread.Sleep(1000);}
	
}
Messaging.Control.Shutdown();
```

Send some messages:
```csharp
Messaging.Sender().SendMessage(new MyExample{Hello = "World"});
```

Notes
-----
* Messaging.Receiver() to get a new `INodeFactory` instance (this uses StructureMap under the hood)
* Creating listener nodes takes time and resources. Do it infrequently -- usually once at the start of your app.
* Your handler will get `new()`'d for every message. Don't do anything complex in the handler constructor!
* To listen to messages, `factory.Listener().Handle<IMyMessageInterface>().With<MyHandlerType>()`
* Each listener can handle any number of message => handler pairs, and a message can have more than one handler (they all fire in parallel)

See further simple examples in the Integration Tests
