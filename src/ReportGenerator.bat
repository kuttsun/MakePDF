@echo off

set REPORTGEN="packages\ReportGenerator.3.1.2\tools\ReportGenerator.exe"

set OUTPUT="OpenCover.xml"

set OUTPUT_DIR="ReportGenerator"

%REPORTGEN% -reports:%~dp0%OUTPUT% -targetdir:%~dp0%OUTPUT_DIR%
