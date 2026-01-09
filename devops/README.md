# Deployment Scripts

Scripts for building and deploying the Docker image to the remote server.

## Setup

1. **Create deployment configuration:**
   ```bash
   copy deploy-config.example.bat deploy-config.bat
   ```
   Edit `deploy-config.bat` with your SSH server details.

2. **Verify SSH key authentication:**
   ```bash
   ssh your-user@your-server
   ```
   Should connect without password prompt.

## Usage

### Full Build and Deploy
```bash
build-and-deploy.bat
```

### Build Only (create tar file)
```bash
build.bat
```

### Deploy Only (requires tar file from build)
```bash
deploy.bat
```

## What the Scripts Do

### build.bat
1. Builds Docker image with version tag
2. Saves image to tar file

### deploy.bat
1. Copies tar file to remote server via SCP
2. Removes old `:latest` container and image
3. Loads new image and tags it as `:latest`
4. Starts containers with docker compose
5. Cleans up local tar file

### build-and-deploy.bat
Runs `build.bat` followed by `deploy.bat`.

## Version Management

Version is read from `../VERSION` file:
```
CURRENT=x.y.z
```

The `/version` endpoint also reads from this file.

## Files

- `build.bat` - Build Docker image and save to tar file
- `deploy.bat` - Deploy tar file to remote server
- `build-and-deploy.bat` - Run build and deploy sequentially
- `deploy-config.example.bat` - Template configuration file
- `deploy-config.bat` - Your configuration (not in git)
- `docker-compose.yml` - Docker compose file for the remote server

## Troubleshooting

**Permission denied during SCP:**
- Ensure your user has sudo privileges on the server

**SSH connection failed:**
- Verify SSH_HOST and SSH_PORT in config
- Check that SSH service is running on server

**Docker deployment failed:**
- Check that docker and docker compose are installed on server
- Verify docker-compose.yml exists in DEPLOY_PATH
