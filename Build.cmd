@echo off
cd %~dp0

rake -f build\build.rb "build:no_test_build[., SevenDigital.Messaging.sln, build, full, local, Release]" 
pause
