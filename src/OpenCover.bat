set MSTEST=%1

set OPENCOVER="%~dp0packages\OpenCover.4.6.519\tools\OpenCover.Console.exe"

set TARGET_TEST="MakePdf.Core.Tests.dll"

set TARGET_DIR=MakePdf.Core.Tests\bin\Release

rem OpenCover output file
set OUTPUT=OpenCover.xml

set FILTERS="+[*]*"

%OPENCOVER% -register:user -target:%MSTEST% -targetargs:%TARGET_TEST% -targetdir:"%~dp0%TARGET_DIR%" -filter:%FILTERS% -output:"%~dp0%OUTPUT%"
