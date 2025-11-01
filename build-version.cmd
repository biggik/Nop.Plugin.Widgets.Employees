@echo off
set sln_file=Nop.Plugin.Widgets.Employees

cd %1

dotnet restore

cd "%sln_file%"
dotnet build "%sln_file%.csproj" --configuration=Debug --no-restore

echo Starting Release build...
dotnet build "%sln_file%.csproj" --configuration=Release --no-restore

cd ..
cd ..
