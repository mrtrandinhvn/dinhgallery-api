using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;

namespace DinhGalleryApi.UnitTests.Queries;

public class GallerySearchQueryTests
{
    [Theory]
    [InlineData("mh", "@DisplayName:*mh*")]
    [InlineData("summer photos", "@DisplayName:*summer\\ photos*")]
    [InlineData("photo*", "@DisplayName:*photo\\**")]
    public void BuildFolderNameContainsQuery_UsesContainsWildcardsAndEscapesInput(
        string searchText,
        string expectedQuery)
    {
        string query = GallerySearchQuery.BuildFolderNameContainsQuery(searchText);

        Assert.Equal(expectedQuery, query);
    }
}
