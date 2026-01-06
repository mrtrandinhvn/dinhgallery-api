using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Queries.Repositories;
using dinhgallery_api.Infrastructures;
using Microsoft.Extensions.Logging;
using Moq;

namespace DinhGalleryApi.UnitTests.Services;

public class GalleryCommandServiceTests
{
    private readonly Mock<ILogger<GalleryCommandService>> _mockLogger;
    private readonly Mock<IGalleryFolderWriteRepository> _mockFolderRepository;
    private readonly Mock<IGalleryFileWriteRepository> _mockFileRepository;
    private readonly Mock<IGalleryQueryRepository> _mockQueryRepository;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<IVideoProcessingService> _mockVideoProcessingService;
    private readonly GalleryCommandService _service;

    public GalleryCommandServiceTests()
    {
        _mockLogger = new Mock<ILogger<GalleryCommandService>>();
        _mockFolderRepository = new Mock<IGalleryFolderWriteRepository>();
        _mockFileRepository = new Mock<IGalleryFileWriteRepository>();
        _mockQueryRepository = new Mock<IGalleryQueryRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockVideoProcessingService = new Mock<IVideoProcessingService>();

        _service = new GalleryCommandService(
            _mockLogger.Object,
            _mockFolderRepository.Object,
            _mockFileRepository.Object,
            _mockQueryRepository.Object,
            _mockStorageService.Object,
            _mockVideoProcessingService.Object
        );
    }

    [Fact]
    public async Task UpdateFolderDisplayNameAsync_WithValidInput_ShouldReturnTrue()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string displayName = "New Folder Name";

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.Is<UpdateFolderDisplayNameInput>(
            input => input.FolderId == folderId && input.DisplayName == displayName)))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, displayName);

        // Assert
        Assert.True(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFolderDisplayNameAsync_WithEmptyDisplayName_ShouldReturnFalse()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string emptyDisplayName = "";

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, emptyDisplayName);

        // Assert
        Assert.False(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFolderDisplayNameAsync_WithWhitespaceDisplayName_ShouldReturnFalse()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string whitespaceDisplayName = "   ";

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, whitespaceDisplayName);

        // Assert
        Assert.False(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFolderDisplayNameAsync_WithNullDisplayName_ShouldReturnFalse()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string? nullDisplayName = null;

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, nullDisplayName!);

        // Assert
        Assert.False(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFolderDisplayNameAsync_WhenRepositoryReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string displayName = "New Folder Name";

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()))
            .ReturnsAsync(false);

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, displayName);

        // Assert
        Assert.False(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Once);
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Folder 123")]
    [InlineData("My Photos 2026")]
    [InlineData("Special!@#$%")]
    public async Task UpdateFolderDisplayNameAsync_WithVariousValidNames_ShouldReturnTrue(string displayName)
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _service.UpdateFolderDisplayNameAsync(folderId, displayName);

        // Assert
        Assert.True(result);
    }
}
