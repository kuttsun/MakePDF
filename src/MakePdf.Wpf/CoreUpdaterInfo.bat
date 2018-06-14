cd %~dp0
set TOOL=%~dp0..\..\..\CoreUpdater\src\CoreUpdater.Console\bin\Release\netcoreapp2.0\CoreUpdater.Console.dll
set TARGET=%~dp0bin\MakePdf
set NAME=MakePdf
set VERSION=0.15.0
dotnet %TOOL% -d=%TARGET% -n=%NAME% -v=%VERSION%