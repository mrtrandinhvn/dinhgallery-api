@echo off
echo ========================================
echo Docker Build Script
echo ========================================
echo.

REM Load configuration
if not exist deploy-config.bat (
    echo ERROR: deploy-config.bat not found!
    echo Please copy deploy-config.example.bat to deploy-config.bat and configure your SSH server details.
    pause
    exit /b 1
)

call deploy-config.bat

REM Load version from VERSION file
if not exist ..\VERSION (
    echo ERROR: VERSION file not found!
    pause
    exit /b 1
)
for /f "tokens=1,2 delims==" %%a in (..\VERSION) do (
    if "%%a"=="CURRENT" set NEW_VERSION=%%b
)

echo Make sure that docker is running first
echo.

REM Build Docker image (from parent directory)
echo [1/2] Building Docker image %IMAGE_NAME%:%NEW_VERSION%...
cd ..
docker build -t %IMAGE_NAME%:%NEW_VERSION% -f ./DinhGalleryApi/Dockerfile .
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker build failed!
    cd devops
    pause
    exit /b 1
)
cd devops

REM Save Docker image to tar
echo [2/2] Saving Docker image to tar file...
docker save %IMAGE_NAME%:%NEW_VERSION% > %IMAGE_NAME%_%NEW_VERSION%.tar
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Docker save failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build completed: %IMAGE_NAME%_%NEW_VERSION%.tar
echo Run deploy.bat to deploy to server
echo ========================================
if not defined SKIP_PAUSE pause
