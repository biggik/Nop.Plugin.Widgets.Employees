@echo off
set sln_file=Nop.Plugin.Widgets.Employees

cd 4.30
dotnet build %sln_file% --configuration=Release
dotnet build %sln_file% --configuration=Debug

cd ..\4.20
dotnet build %sln_file% --configuration=Release
dotnet build %sln_file% --configuration=Debug

cd ..