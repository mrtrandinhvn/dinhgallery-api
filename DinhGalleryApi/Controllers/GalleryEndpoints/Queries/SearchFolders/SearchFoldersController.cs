using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.SearchFolders;

[ApiController]
[Route("gallery/search")]
public class SearchFoldersController : ControllerBase
{
    private readonly IQueryHandler<SearchFoldersQuery, List<FolderDetailsReadModel>> _handler;

    public SearchFoldersController(IQueryHandler<SearchFoldersQuery, List<FolderDetailsReadModel>> handler)
    {
        _handler = handler;
    }

    /// <summary>Searches folder display names, returning at most 10 most recently updated matches.</summary>
    [HttpGet]
    [AllowAnonymous]
    public Task<List<FolderDetailsReadModel>> SearchFolders([FromQuery] string searchText = "")
    {
        return _handler.HandleAsync(new SearchFoldersQuery { SearchText = searchText });
    }
}
