@echo off
set sln_file=Nop.Plugin.Widgets.Employees

cd %1
dotnet build %sln_file% --configuration=Debug -- no-dependencies
dotnet build %sln_file% --configuration=Release --no-dependencies

cd ..
