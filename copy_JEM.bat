@echo off
robocopy ..\JEM\deploy libs JEM.Core.dll JEM.Core.xml JEM.Core.dll.mdb JEM.Core.pdb
robocopy ..\JEM\deploy libs Newtonsoft.Json.dll Newtonsoft.Json.xml
exit 0