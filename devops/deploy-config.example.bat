@echo off
REM ========================================
REM SSH Server Configuration Template
REM ========================================
REM Copy this file to deploy-config.bat and fill in your actual values
REM The entire devops/ folder is in .gitignore to prevent committing credentials

REM SSH Server Details
set SSH_USER=your-username
set SSH_HOST=your-server-ip-or-hostname
set SSH_PORT=22

REM Remote Deployment Path
set DEPLOY_PATH=/root/dinh-gallery-api

REM Docker Image Details (version is read from ../VERSION file)
set IMAGE_NAME=dinhgalleryapi

REM API URL for version verification
set VERSION_URL=https://your-api-domain/version

REM Optional: Uncomment and set if using non-default SSH key
REM set SSH_KEY_PATH=C:\Users\YourUser\.ssh\id_rsa
