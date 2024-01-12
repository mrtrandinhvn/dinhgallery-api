# dinhgallery-api
Web api for https://gallery.dinhtran.page. Use Azure Active Directory for authentication.

## Technologies

### Environment
- .NET 8 on linux Azure App Service.

### Authentication & Authorization
- Azure Active Directory.
- MSAL 2.0.
- Enterprise Application roles.

## Setup locally
### Launch with Docker
- Install Docker for windows.
- Check `launchSettings.json` to find the `env.example` file, copy that file to the path set in `DockerfileRunArguments` and fill in the value.  