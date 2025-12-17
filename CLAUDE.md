# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**dinhgallery-api** is the backend service for the Dinh Gallery website (https://gallery.dinhtran.page). It's a .NET 10 Web API that runs in Docker on a Linux VPS behind an NGINX reverse proxy.

## Commands

### Build and Test
```bash
# Build the solution
dotnet build DinhGallery.Api.sln

# Run tests
dotnet test DinhGalleryApi.UnitTests/DinhGalleryApi.UnitTests.csproj

# Run the API locally (non-Docker)
dotnet run --project DinhGalleryApi/dinhgallery-api.csproj
```

### Docker Development
The project is configured for Docker development. Check `Properties/launchSettings.json` for the Docker profile.

**Setup requirements:**
1. Copy `Properties/env.example` to the path specified in `DockerfileRunArguments` (check `launchSettings.json`)
2. Configure volume mapping for the storage folder where uploaded files are stored
3. Run Docker as root user (required for file system operations)

```bash
# Build Docker image
docker build -t dinhgallery-api .

# Run with Docker (example from launchSettings.json)
docker run --user root --env-file <path-to-env-file> -v <host-storage-path>:/app/storage -p 80:80 dinhgallery-api
```

## Architecture

### Technology Stack
- **.NET 10** Web API
- **Redis** (via Redis.OM) for data storage with JSON document model
- **Azure Active Directory / MSAL 2.0** for authentication
- **File system storage** for uploaded files (served as static files via `/storage` endpoint)

### Authentication & Authorization
- Global fallback policy requires authenticated users with `Admin` role (see Program.cs:47-50)
- Individual endpoints can override with `[AllowAnonymous]` attribute
- JWT bearer tokens via Azure AD Enterprise Application roles
- Role constant defined in `BusinessObjects/Constants/AppRole.cs`

### Data Model
The application uses Redis as a JSON document store (not a cache) with two main entities:

- **FolderDbModel** (`DbModels/FolderDbModel.cs`): Represents gallery folders
  - Uses `Ulid` for IDs
  - Contains `PhysicalFolderName` for file system mapping
  - Indexed by `CreatedAtUtc` and `DisplayName`

- **FileDbModel** (`DbModels/FileDbModel.cs`): Represents individual files
  - Links to folders via `FolderId`
  - Contains `DownloadUri` for file access
  - Indexed by `CreatedAtUtc`, `DisplayName`, and `FolderId`

Redis indexes are created on startup by `IndexCreationService` hosted service.

### Project Structure

**Controllers are organized by CQRS pattern:**
- **Query side** (`Controllers/GalleryEndpoints/Queries/`):
  - `GalleryQueryController` - Endpoints for reading data
  - `GalleryQueryService` - Query business logic
  - `IGalleryQueryRepository` - Data access interface (implemented in `Infrastructures/Repositories/GalleryQueryRepository.cs`)
  - Read models in `Models/` subdirectory

- **Command side** (`Controllers/GalleryEndpoints/Commands/`):
  - `GalleryCommandControler` [sic] - Endpoints for writing data
  - `GalleryCommandService` - Command business logic
  - `IGalleryFileWriteRepository` and `IGalleryFolderWriteRepository` - Write interfaces
  - Implemented in `Infrastructures/Repositories/`

**Storage:**
- Files stored in `storage/` directory at application root
- `FileSystemStorageService` handles file system operations
- Static files served via ASP.NET Core StaticFiles middleware at `/storage` path
- Files organized by folder: `/storage/{physicalFolderName}/{physicalFileName}`

**Configuration:**
- Service registration in `BusinessObjects/IServiceCollectionExtensions.cs`
- Max request body size: 1GB (for large file uploads)
- Required config sections in appsettings.json:
  - `AzureAd` - Azure AD authentication settings
  - `Redis` - Redis connection (Host, Port, Password)
  - `StorageSettings` - Storage service base URL for download URIs
  - `AllowedOrigins` - CORS origins (semicolon-separated)

### Dependency Injection Setup
All service configuration is in `IServiceCollectionExtensions.cs`:
- `ConfigureOptions()` - Configures Kestrel/Form options and binds StorageSettings
- `ConfigureAppServices()` - Registers Redis, repositories, and services
- `ConfigureHostedServices()` - Registers background services (Redis index creation)

### Key Patterns
1. **Ulid for IDs**: All entities use `Ulid` type for identifiers (not Guid)
2. **Physical vs Display names**: Files/folders have both physical names (file system) and display names (user-facing)
3. **URI-based file references**: Files stored as URIs (not paths) in `DownloadUri` field
4. **Hosted service for indexes**: Redis indexes auto-created on app startup
