using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[ApiController]
[Route("gallery")]
public class GalleryQueryController : ControllerBase
{
    private readonly IGalleryQueryRepository _service;

    public GalleryQueryController(IGalleryQueryRepository service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<List<FolderDetailsReadModel>> GetFolderList()
    {
        return _service.GetFolderListAsync();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("folder/{id}")]
    public Task<FolderDetailsReadModel?> GetFolderDetails(Ulid id)
    {
        return _service.GetFolderDetailsAsync(id);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("file/{id}")]
    public async Task<FileDetailsResponse?> GetFileDetails(Ulid id)
    {
        FileDetailsReadModel? fileDetails = await _service.GetFileDetailsAsync(id);
        return fileDetails == null ? null : new FileDetailsResponse(fileDetails);
    }
}