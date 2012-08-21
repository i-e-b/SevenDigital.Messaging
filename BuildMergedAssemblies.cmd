cd %~dp0\binaries

copy "..\src\SevenDigital.Messaging\bin\Release\*.dll" "." /Y

..\tools\ILMerge.exe /allowDup /targetplatform:v4 /out:..\merged\SevenDigital.Messaging.dll SevenDigital.Messaging.Base.dll Magnum.dll MassTransit.dll MassTransit.Transports.RabbitMq.dll RabbitMQ.Client.dll StructureMap.dll 

COPY SevenDigital.Messaging.Types.dll ..\merged\