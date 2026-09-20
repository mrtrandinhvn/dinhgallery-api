@echo off
echo ========================================
echo Docker Deploy Script
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

REM Use the configured health-check URL, or derive it from the version URL for existing configurations.
if "%HEALTH_CHECK_URL%"=="" set HEALTH_CHECK_URL=%VERSION_URL:/version=/health/ready%

REM Load version from VERSION file
if not exist ..\VERSION (
    echo ERROR: VERSION file not found!
    pause
    exit /b 1
)
for /f "tokens=1,2 delims==" %%a in (..\VERSION) do (
    if "%%a"=="CURRENT" set NEW_VERSION=%%b
)

REM Check if tar file exists
if not exist %IMAGE_NAME%_%NEW_VERSION%.tar (
    echo ERROR: %IMAGE_NAME%_%NEW_VERSION%.tar not found!
    echo Run build.bat first to create the tar file.
    pause
    exit /b 1
)

REM Copy tar file to user's home directory
echo [1/5] Copying tar file to SSH server %SSH_USER%@%SSH_HOST%:~/...
scp -P %SSH_PORT% %IMAGE_NAME%_%NEW_VERSION%.tar %SSH_USER%@%SSH_HOST%:~/
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: SCP file transfer failed!
    pause
    exit /b 1
)

REM Move file to deployment directory and deploy
echo [2/5] Moving file and deploying on remote server...
ssh -p %SSH_PORT% %SSH_USER%@%SSH_HOST% "mv ~/%IMAGE_NAME%_%NEW_VERSION%.tar %DEPLOY_PATH%/ && cd %DEPLOY_PATH% && docker container remove %IMAGE_NAME%_latest --force 2>/dev/null || true && docker image rm %IMAGE_NAME%:latest 2>/dev/null || true && docker load -i ./%IMAGE_NAME%_%NEW_VERSION%.tar && docker tag %IMAGE_NAME%:%NEW_VERSION% %IMAGE_NAME%:latest && docker compose up --detach"
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Remote Docker deployment failed!
    pause
    exit /b 1
)

REM Clean up local tar file
echo [3/5] Cleaning up local tar file...
del %IMAGE_NAME%_%NEW_VERSION%.tar

REM Verify deployment by checking version endpoint
echo [4/5] Verifying deployed version...
timeout /t 3 /nobreak > nul
echo.
echo Deployed version (from %VERSION_URL%):
curl -s %VERSION_URL%
echo.

REM Poll readiness because the container may need time to start and connect to Redis.
echo [5/5] Waiting for application readiness at %HEALTH_CHECK_URL%...
setlocal EnableDelayedExpansion
set /a HEALTH_CHECK_ATTEMPT=0

:health_check
set /a HEALTH_CHECK_ATTEMPT+=1
curl -fsS --max-time 10 %HEALTH_CHECK_URL%
if not errorlevel 1 goto health_check_passed

if !HEALTH_CHECK_ATTEMPT! GEQ 6 goto health_check_failed
echo Health check attempt !HEALTH_CHECK_ATTEMPT! failed. Retrying in 5 seconds...
timeout /t 5 /nobreak > nul
goto health_check

:health_check_passed
echo.
echo Application is ready.
goto health_check_complete

:health_check_failed
echo.
echo ERROR: Application did not become ready after !HEALTH_CHECK_ATTEMPT! attempts.
endlocal
exit /b 1

:health_check_complete
endlocal

echo.
echo ========================================
echo Deployment completed successfully!
echo ========================================
pause
