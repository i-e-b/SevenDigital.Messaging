SevenDigital.Messaging
======================

A distributed contracts-based sender/handler messaging system built on RabbitMQ and MassTransit

Parts
=====

SevenDigital.Messaging
----------------------
All of the below wrapped up in a single dll package.
You *MUST* call `ConfigureMessaging` before using any of it's parts.
If you don't want the one-dll and structuremap stuff, use the parts you want from below:

SevenDigital.Messaging.Core
---------------------------
The actual messaging system! An abstraction over RabbitMQ / MassTransit. Your starting point is INodeFactory.

SevenDigital.Messaging.StructureMap
-----------------------------------
A configuration helper for .Messaging which makes it easy to get up and running.
Call `ConfigureMessaging.WithDefaults().AndMessagingServer("my.rabbitserver.com")` when your app starts.

SevenDigital.Messaging.Types
----------------------------
Holds the IMessage contract separately, so you don't have to include the whole system to define contracts.
