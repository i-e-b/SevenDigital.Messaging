cd %~dp0
mkdir binaries
mkdir merged
cd binaries

copy "..\src\SevenDigital.Messaging.Base\bin\Release\*.dll" "." /Y
copy "..\src\Messaging.Management\bin\Release\SevenDigital.Messaging.Management.dll" "."

..\tools\ILMerge.exe /allowDup /targetplatform:v4 /out:..\merged\SevenDigital.Messaging.dll SevenDigital.Messaging.Base.dll Magnum.dll MassTransit.dll MassTransit.Transports.RabbitMq.dll RabbitMQ.Client.dll StructureMap.dll 

COPY SevenDigital.Messaging.Types.dll ..\merged\
COPY SevenDigital.Messaging.Management.dll ..\merged\
