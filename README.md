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
  - Basic server-side handling

### Authentication & Authorization
- **Azure Active Directory**  
- **MSAL 2.0**  
- **Enterprise Application roles** for permission management



## Setup locally
### Launch with Docker
- Install Docker for windows.
- Check `launchSettings.json` to find the `env.example` file, copy that file to the path set in `DockerfileRunArguments` and fill in the value.  
