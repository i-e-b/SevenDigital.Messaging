SevenDigital.Messaging
======================

A distributed contracts-based sender/handler messaging system built on RabbitMQ and MassTransit

Path of Least Resistence
========================
* Install Erlang [http://www.erlang.org/download.html] and RabbitMQ server [http://www.rabbitmq.com/download.html]
* Add all references from `/binaries`, call `ConfigureMessaging.WithDefaults()` in your app startup.
* Use ObjectFactory to get a new `INodeFactory` instance
* To listen to messages, `factory.Listener().Handle<IMyMessageInterface>().With<MyHandlerType>()`
* To send a message, `factory.Sender().SendMessage(new MyMessage())`

Notes
-----
* Creating nodes takes time and resources. Do it infrequently.
* Your handler will get `new()`'d for every message. Don't do heavy things in the handler!

Moving Parts
============

binaries
--------
These are all the projects and their dependencies.
You may wish to pull the non-SevenDigital dependencies from NuGet.

SevenDigital.Messaging
----------------------
The core abstraction over RabbitMQ / MassTransit. Your starting point is INodeFactory.

SevenDigital.Messaging.StructureMap
-----------------------------------
A configuration helper for .Messaging which makes it easy to get up and running.
Call `ConfigureMessaging.WithDefaults().AndMessagingServer("my.rabbitserver.com")` when your app starts.

SevenDigital.Messaging.Types
----------------------------
Holds the IMessage contract separately, so you don't have to include the whole system to define contracts.
