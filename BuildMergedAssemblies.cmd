cd %~dp0
mkdir binaries
mkdir merged
cd binaries

copy "..\src\SevenDigital.Messaging\bin\Release\*.dll" "." /Y

..\tools\ILMerge.exe /allowDup /targetplatform:"v4,c:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" /out:..\merged\SevenDigital.Messaging.dll SevenDigital.Messaging.dll RabbitMQ.Client.dll StructureMap.dll SevenDigital.Messaging.Base.dll SignalHandling.dll DiskQueue.dll

COPY SevenDigital.Messaging.Types.dll ..\merged\
