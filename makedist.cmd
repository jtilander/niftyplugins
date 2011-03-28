@echo off
setlocal

set VERSION=%1

if "%1" == "" (
	echo Usage: makedist.cmd ^<version number^>
	goto :EOF
)


python bin\build.py %VERSION%


endlocal
