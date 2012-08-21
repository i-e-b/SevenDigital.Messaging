@echo off
cd %~dp0..

rake -f Build\build-win.rb "build:no_test_build[., SevenDigital.Messaging.sln, build, full, local, Release]" 
