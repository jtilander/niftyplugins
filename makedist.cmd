@echo off
setlocal

set VERSION=%1

if "%1" == "" (
	echo Usage: makedist.cmd ^<version number^>
	goto :EOF
)

rem http://stackoverflow.com/questions/8648428/an-error-occurred-while-validating-hresult-8000000a
regedit /s bin\msbuildfix.reg


python bin\build.py %VERSION%

endlocal
