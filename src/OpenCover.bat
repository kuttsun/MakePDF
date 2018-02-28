set MSTEST=%1

set OPENCOVER="packages\OpenCover.4.6.519\tools\OpenCover.Console.exe"

set TARGET_TEST="MakePdf.Core.Tests.dll"

set TARGET_DIR=".\MakePdf.Core.Tests\bin\Release"

rem OpenCover output file
set OUTPUT="OpenCover.xml"

rem set FILTERS="+[OpenCoverSample]*"
set FILTERS="+[*]*"

%OPENCOVER% -register:user -target:%MSTEST% -targetargs:%TARGET_TEST% -targetdir:%TARGET_DIR% -filter:%FILTERS% -output:%OUTPUT%
