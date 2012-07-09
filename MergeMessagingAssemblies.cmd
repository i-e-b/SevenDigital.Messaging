cd %~dp0\binaries

..\tools\ILMerge.exe /allowDup /targetplatform:v4 /out:..\merged\SevenDigital.Messaging.dll SevenDigital.Messaging.dll Magnum.dll MassTransit.dll MassTransit.Transports.RabbitMq.dll RabbitMQ.Client.dll StructureMap.dll 

COPY SevenDigital.Messaging.Types.dll ..\merged\