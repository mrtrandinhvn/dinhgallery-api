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
echo [1/4] Copying tar file to SSH server %SSH_USER%@%SSH_HOST%:~/...
scp -P %SSH_PORT% %IMAGE_NAME%_%NEW_VERSION%.tar %SSH_USER%@%SSH_HOST%:~/
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: SCP file transfer failed!
    pause
    exit /b 1
)

REM Move file to deployment directory with sudo and deploy
echo [2/4] Moving file and deploying on remote server...
ssh -p %SSH_PORT% %SSH_USER%@%SSH_HOST% "sudo mv ~/%IMAGE_NAME%_%NEW_VERSION%.tar %DEPLOY_PATH%/ && cd %DEPLOY_PATH% && sudo docker container remove %IMAGE_NAME%_latest --force 2>/dev/null || true && sudo docker image rm %IMAGE_NAME%:latest 2>/dev/null || true && sudo docker load -i ./%IMAGE_NAME%_%NEW_VERSION%.tar && sudo docker tag %IMAGE_NAME%:%NEW_VERSION% %IMAGE_NAME%:latest && sudo docker compose up --detach"
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Remote Docker deployment failed!
    pause
    exit /b 1
)

REM Clean up local tar file
echo [3/4] Cleaning up local tar file...
del %IMAGE_NAME%_%NEW_VERSION%.tar

REM Verify deployment by checking version endpoint
echo [4/4] Verifying deployment...
timeout /t 3 /nobreak > nul
echo.
echo Deployed version (from %VERSION_URL%):
curl -s %VERSION_URL%
echo.

echo.
echo ========================================
echo Deployment completed successfully!
echo ========================================
pause
