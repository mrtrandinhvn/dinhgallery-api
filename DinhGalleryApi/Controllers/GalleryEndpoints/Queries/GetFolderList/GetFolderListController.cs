using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

[ApiController]
[Route("gallery")]
public class GetFolderListController : ControllerBase
{
    private readonly IQueryHandler<GetFolderListQuery, PaginatedResult<FolderDetailsReadModel>> _handler;

    public GetFolderListController(
        IQueryHandler<GetFolderListQuery, PaginatedResult<FolderDetailsReadModel>> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets a paginated list of folders in the gallery
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public Task<PaginatedResult<FolderDetailsReadModel>> GetFolderList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetFolderListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return _handler.HandleAsync(query);
    }
}
