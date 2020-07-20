@echo off
set project=Nop.Plugin.Widgets.Employees

cd 4.30\%project%
Echo building 4.30 of %project%
rd ..\_build\release\. /s /q > 0
dotnet build %project%.csproj --configuration=Release --no-incremental

cd ..\..\4.20\%project%
Echo building 4.20 of %project%
rd ..\_build\release\. /s /q > 0
dotnet build %project%.csproj --configuration=Release --no-incremental

cd ..\..
