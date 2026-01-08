using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFileDetails;

[ApiController]
[Route("gallery")]
public class GetFileDetailsController : ControllerBase
{
    private readonly IQueryHandler<GetFileDetailsQuery, FileDetailsResponse?> _handler;

    public GetFileDetailsController(
        IQueryHandler<GetFileDetailsQuery, FileDetailsResponse?> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets details of a specific file by ID
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [Route("file/{id}")]
    public Task<FileDetailsResponse?> GetFileDetails(Ulid id)
    {
        var query = new GetFileDetailsQuery
        {
            FileId = id
        };

        return _handler.HandleAsync(query);
    }
}
