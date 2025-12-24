using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries;

[ApiController]
[Route("gallery")]
public class GetFileDetailsController : ControllerBase
{
    private readonly IGalleryQueryRepository _service;

    public GetFileDetailsController(IGalleryQueryRepository service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets details of a specific file by ID
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [Route("file/{id}")]
    public async Task<FileDetailsResponse?> GetFileDetails(Ulid id)
    {
        FileDetailsReadModel? fileDetails = await _service.GetFileDetailsAsync(id);
        return fileDetails == null ? null : new FileDetailsResponse(fileDetails);
    }
}
