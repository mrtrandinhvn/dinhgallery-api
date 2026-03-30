# dinhgallery-api

The **dinhgallery-api** is the backend service for the Dinh Gallery website (https://gallery.dinhtran.page). It uses Azure Active Directory for secure sign-in and access control.

## Overview

This project runs a **.NET 10 Web API** inside a Docker container and is hosted on a Linux VPS. An **NGINX** reverse proxy sits in front of the API to handle routing and SSL.

## Architecture & Technologies

### Runtime Environment
- **.NET 10** Web API  
- **Docker** container  
- **Linux VPS** deployment  
- **NGINX** reverse proxy for:
  - Routing requests to the API  
  - Managing HTTPS  
  - Serve static files

### Authentication & Authorization
- **Azure Active Directory**  
- **MSAL 2.0**  
- **Enterprise Application roles** for permission management


## Setup locally
### Launch with Docker
- Install Docker for windows.
- Check `launchSettings.json` to find the `env.example` file, copy that file to the path set in `DockerfileRunArguments` and fill in the value.  
- Double check the volume mapping for storage folder. That is where all the user uploaded files are going to be.

## Health Endpoints

The API exposes separate liveness and readiness endpoints:

- `GET /health/live`
  Returns success when the application process is running. This endpoint does not check external dependencies.

- `GET /health/ready`
  Returns success only when the application is ready to serve requests. This currently includes a Redis connectivity check.

- `GET /version`
  Returns the application version from the `VERSION` file. This is useful for deployment verification, but it is not a readiness check because it does not validate Redis or other dependencies.

## DevOps Scripts

The [devops](/d:/projects/dinhgallery-api/devops) folder contains the deployment scripts and related documentation:

- `build.bat` builds the Docker image and exports it as a tar file.
- `deploy.bat` copies the tar file to the remote server and deploys it.
- `build-and-deploy.bat` runs build and deploy in sequence.
- `deploy-config.example.bat` is the template for local deployment configuration.
- `README.md` inside `devops/` documents the deployment workflow in more detail.
