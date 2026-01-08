using dinhgallery_api.Controllers.GalleryEndpoints.Commands.Repositories;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;
using Microsoft.Extensions.Logging;
using Moq;

namespace DinhGalleryApi.UnitTests.Handlers;

public class UpdateFolderDisplayNameCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateFolderDisplayNameCommandHandler>> _mockLogger;
    private readonly Mock<IGalleryFolderWriteRepository> _mockFolderRepository;
    private readonly UpdateFolderDisplayNameCommandHandler _handler;

    public UpdateFolderDisplayNameCommandHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UpdateFolderDisplayNameCommandHandler>>();
        _mockFolderRepository = new Mock<IGalleryFolderWriteRepository>();
        _handler = new UpdateFolderDisplayNameCommandHandler(_mockLogger.Object, _mockFolderRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCallRepositoryAndReturnTrue()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string displayName = "New Folder Name";
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = folderId,
            DisplayName = displayName
        };

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.Is<UpdateFolderDisplayNameInput>(
            input => input.FolderId == folderId && input.DisplayName == displayName)))
            .ReturnsAsync(true);

        // Act
        bool result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = "New Folder Name"
        };

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()))
            .ReturnsAsync(false);

        // Act
        bool result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Once);
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Folder 123")]
    [InlineData("My Photos 2026")]
    [InlineData("Special!@#$%")]
    public async Task HandleAsync_WithVariousDisplayNames_ShouldCallRepositoryAndReturnResult(string displayName)
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = displayName
        };

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()))
            .ReturnsAsync(true);

        // Act
        bool result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        _mockFolderRepository.Verify(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogInformationWithCorrectParameters()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        string displayName = "Test Folder";
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = folderId,
            DisplayName = displayName
        };

        _mockFolderRepository.Setup(x => x.UpdateAsync(It.IsAny<UpdateFolderDisplayNameInput>()))
            .ReturnsAsync(true);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Updating folder") && v.ToString()!.Contains(displayName)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
