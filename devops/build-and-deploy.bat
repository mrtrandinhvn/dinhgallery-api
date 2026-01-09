@echo off
echo ========================================
echo Docker Build and Deploy Script
echo ========================================
echo.

set SKIP_PAUSE=1
call build.bat
if %ERRORLEVEL% NEQ 0 (
    echo Build failed, aborting deployment.
    exit /b 1
)

call deploy.bat
