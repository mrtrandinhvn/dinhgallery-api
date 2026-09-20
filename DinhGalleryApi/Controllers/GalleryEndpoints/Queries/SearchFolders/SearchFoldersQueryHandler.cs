using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.SearchFolders;

public class SearchFoldersQueryHandler : IQueryHandler<SearchFoldersQuery, List<FolderDetailsReadModel>>
{
    private const int ResultLimit = 10;
    private readonly IGalleryQueryRepository _repository;

    public SearchFoldersQueryHandler(IGalleryQueryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<FolderDetailsReadModel>> HandleAsync(SearchFoldersQuery query, CancellationToken cancellationToken = default)
    {
        string searchText = query.SearchText.Trim();
        return string.IsNullOrEmpty(searchText)
            ? Task.FromResult(new List<FolderDetailsReadModel>())
            : _repository.SearchFoldersByNameAsync(searchText, ResultLimit);
    }
}
