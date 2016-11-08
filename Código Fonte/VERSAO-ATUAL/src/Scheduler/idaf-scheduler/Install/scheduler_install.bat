@ECHO off

Path=C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow;C:\Program Files (x86)\Microsoft SDKs\F#\3.1\Framework\v4.0\;C:\Program Files (x86)\Microsoft SDKs\TypeScript\1.0;C:\Program Files (x86)\MSBuild\12.0\bin;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\BIN;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools;C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\VCPackages;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools;C:\Program Files (x86)\Windows Kits\8.1\bin\x86;C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\;%Path%

"C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild" ..\..\idaf-scheduler.sln /t:Build /p:Configuration=Release /fileLogger

if %errorlevel% neq 0 pause
if %errorlevel% neq 0 exit /b

FOR /D %%f IN ("..\bin\Release\*") do rd %%f /s /q

del ..\bin\Release\*.pdb

del ..\bin\Release\*.xml

echo Criando o instalador...

"C:\Program Files (x86)\NSIS\makensis.exe" /O"SetupIDAFScheduler.log" /DSERVICE_NAME="IDAFScheduler" /DPRODUCT_DIR="IDAF Scheduler" /DPRODUCT_EXE="Tecnomapas.EtramiteX.Scheduler.exe" /DPRODUCT_NAME="IDAF Scheduler" /DSOURCE_DIR="..\bin\Release" /DOUTFILE="SetupIDAFScheduler" scheduler_install.nsi
