@echo off
set project=Nop.Plugin.Widgets.Employees

cd 4.40\%project%
Echo building 4.40 of %project%
rd ..\_build\. /s /q > nul
dotnet build %project%.csproj --configuration=Debug --no-incremental
dotnet build %project%.csproj --configuration=Release --no-incremental

cd ..\..\4.30\%project%
Echo building 4.30 of %project%
rd ..\_build\. /s /q > nul
dotnet build %project%.csproj --configuration=Debug --no-incremental
dotnet build %project%.csproj --configuration=Release --no-incremental

REM cd ..\..\4.20\%project%
REM Echo building 4.20 of %project%
REM rd ..\_build\. /s /q > nul
REM dotnet build %project%.csproj --configuration=Debug --no-incremental
REM dotnet build %project%.csproj --configuration=Release --no-incremental

cd ..\..

..\nopPackager\nopPackager\bin\Debug\nopPackager.exe c:\development\Status\nopCommerce-Plugins\%project%
