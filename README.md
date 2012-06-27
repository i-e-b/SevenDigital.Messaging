SevenDigital.Messaging
======================

A distributed contracts-based sender/handler messaging system built on RabbitMQ and MassTransit

Parts
=====

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
