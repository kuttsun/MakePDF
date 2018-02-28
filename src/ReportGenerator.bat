set REPORTGEN="packages\ReportGenerator.3.1.2\tools\ReportGenerator.exe"

set OUTPUT="OpenCover.xml"

set OUTPUT_DIR="html"

%REPORTGEN% -reports:%OUTPUT% -targetdir:%OUTPUT_DIR%
