@echo off
:: 
:: Postbuild step for NiftySolution toolbar.
::

if "%1" == "" (
    goto :HELP
)

if "%2" == "" (
    goto :HELP
)

goto :DOIT

:HELP
    echo Usage: ^<projectdir^> ^<outdir^>
    echo.
    echo Needs to be called from Visual Studio
    goto :EOF

:DOIT
    setlocal

        set PROJECTDIR=%1
        set OUTDIR=%2

        :: Guess where the resgen.exe and al.exe files are...
        :: You might need to change this to suit your environment!!!
        set SDKDIR=%ProgramFiles%\Microsoft SDKs\Windows\v7.0A

        set PATH=%PATH%;%SDKDIR%\Bin\NETFX 4.0 Tools;%SDKDIR%\Bin


        pushd "%PROJECTDIR%"
            mkdir "%PROJECTDIR%\%OUTDIR%\en-US"
            Resgen "%PROJECTDIR%\ToolbarIcons.resx"
            Al /embed:"%PROJECTDIR%\ToolbarIcons.resources" /culture:en-US /out:"%PROJECTDIR%\%OUTDIR%\en-US\NiftySolution.resources.dll"
            del "%PROJECTDIR%\ToolbarIcons.resources"
        popd
    endlocal
