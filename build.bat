@echo off
setlocal

if "%~1"=="" (
    echo Usage: build.bat image-file
    exit /b 1
)

set "IMAGE_PATH=%~1"
set "RESOURCE_DIR=BouncingLogo\sudowoodo.png"
set "OUTPUT_DIR=BouncingLogo\bin\Release\net48"

if not exist "%IMAGE_PATH%" (
    echo Error: File "%IMAGE_PATH%" not found!
    exit /b 1
)

if not exist "%RESOURCE_DIR%" mkdir "%RESOURCE_DIR%"

copy "%IMAGE_PATH%" "%RESOURCE_DIR%" >nul

dotnet build BouncingLogo.sln -c Release

endlocal
