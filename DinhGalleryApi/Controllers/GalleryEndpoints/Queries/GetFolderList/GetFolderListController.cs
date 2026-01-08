using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFolderList;

[ApiController]
[Route("gallery")]
public class GetFolderListController : ControllerBase
{
    private readonly IQueryHandler<GetFolderListQuery, List<FolderDetailsReadModel>> _handler;

    public GetFolderListController(
        IQueryHandler<GetFolderListQuery, List<FolderDetailsReadModel>> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Gets a list of all folders in the gallery
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public Task<List<FolderDetailsReadModel>> GetFolderList()
    {
        var query = new GetFolderListQuery();
        return _handler.HandleAsync(query);
    }
}
