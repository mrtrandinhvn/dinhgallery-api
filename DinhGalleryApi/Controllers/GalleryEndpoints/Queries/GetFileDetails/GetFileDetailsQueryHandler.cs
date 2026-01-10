using dinhgallery_api.BusinessObjects.Queries;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Models;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace dinhgallery_api.Controllers.GalleryEndpoints.Queries.GetFileDetails;

/// <summary>
/// Handler for the GetFileDetailsQuery.
/// </summary>
public class GetFileDetailsQueryHandler
    : IQueryHandler<GetFileDetailsQuery, FileDetailsResponse?>
{
    private readonly ILogger<GetFileDetailsQueryHandler> _logger;
    private readonly IGalleryQueryRepository _repository;

    public GetFileDetailsQueryHandler(
        ILogger<GetFileDetailsQueryHandler> logger,
        IGalleryQueryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<FileDetailsResponse?> HandleAsync(
        GetFileDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Retrieving file details for {FileId}",
            query.FileId);

        FileDetailsReadModel? fileDetails = await _repository.GetFileDetailsAsync(query.FileId);
        return fileDetails is null ? null : FileDetailsResponse.FromReadModel(fileDetails);
    }
}
