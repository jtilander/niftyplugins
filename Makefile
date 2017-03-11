VERSION?=2.0.2
SHELL = cmd.exe
MSBUILD = MSBuild.exe
PATH := $(PATH);C:\Program Files (x86)\MSBuild\14.0\bin\amd64


all: build

build:
	cmd /c $(MSBUILD) NiftyPlugins.sln /maxcpucount /nologo /v:m /t:Build /p:Configuration=Release /p:Platform="Any CPU"

clean:

dist:
	makedist.cmd $(VERSION)
