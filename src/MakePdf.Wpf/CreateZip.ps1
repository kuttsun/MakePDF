# run in the project root

# Arg[0] : Source directory (e.g. .\bin\Release)
# Arg[1] : Destinate directory (e.g. .\bin)
# Arg[2] : Application name (e.g. $(SolutionName))

using namespace System.IO;
using namespace System.IO.Compression;

# source directory path (Release directory)
$sourceDir = $Args[0]
if (-not ($sourceDir)) { return 100 }

# target zip file path
$targetZipFile = $Args[1] + "\" + $Args[2] + ".zip"
if (-not ($targetZipFile)) { return 101 }

# get directory name from target zip file
$parent = [Path]::GetDirectoryName($targetZipFile)
# if there is no directory for target zip file, create a new directory
[Directory]::CreateDirectory($parent)
# if zip file exists, delete it
[File]::Delete($targetZipFile)

# temporary directory name
$tempDir = $parent + "\" + $Args[2]
# delete temporary directory
Remove-Item -Recurse -path $tempDir
# copy to temporary directory from release directory
Copy-Item $sourceDir -destination $tempDir -recurse

# delete unnecessary file from temporary directory
Remove-Item -Recurse -path $tempDir\logs
Remove-Item -Recurse -path $tempDir -include *.pdb
#Remove-Item -Recurse -path $tempDir -include *.xml
#Remove-Item -Recurse -path $tempDir -include *.config -Exclude NLog.config

# read assembly
Add-Type -AssemblyName System.IO.Compression.FileSystem

# create zip
[ZipFile]::CreateFromDirectory($tempDir, $targetZipFile)


