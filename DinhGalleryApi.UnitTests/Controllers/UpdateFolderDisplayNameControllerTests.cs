using dinhgallery_api.Controllers.GalleryEndpoints.Commands;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace DinhGalleryApi.UnitTests.Controllers;

public class UpdateFolderDisplayNameControllerTests
{
    private readonly Mock<IGalleryCommandService> _mockCommandService;
    private readonly UpdateFolderDisplayNameController _controller;

    public UpdateFolderDisplayNameControllerTests()
    {
        _mockCommandService = new Mock<IGalleryCommandService>();
        _controller = new UpdateFolderDisplayNameController(_mockCommandService.Object);
    }

    [Fact]
    public async Task UpdateFolderDisplayName_WithValidRequest_ShouldCallServiceAndReturnResult()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        UpdateFolderDisplayNameRequest request = new()
        {
            DisplayName = "New Folder Name"
        };

        _mockCommandService.Setup(x => x.UpdateFolderDisplayNameAsync(folderId, request.DisplayName))
            .ReturnsAsync(true);

        // Act
        bool result = await _controller.UpdateFolderDisplayName(folderId, request);

        // Assert
        Assert.True(result);
        _mockCommandService.Verify(x => x.UpdateFolderDisplayNameAsync(folderId, request.DisplayName), Times.Once);
    }

    [Fact]
    public async Task UpdateFolderDisplayName_WhenServiceReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        UpdateFolderDisplayNameRequest request = new()
        {
            DisplayName = "New Folder Name"
        };

        _mockCommandService.Setup(x => x.UpdateFolderDisplayNameAsync(folderId, request.DisplayName))
            .ReturnsAsync(false);

        // Act
        bool result = await _controller.UpdateFolderDisplayName(folderId, request);

        // Assert
        Assert.False(result);
        _mockCommandService.Verify(x => x.UpdateFolderDisplayNameAsync(folderId, request.DisplayName), Times.Once);
    }

    [Theory]
    [InlineData("Album 2026")]
    [InlineData("My Photos")]
    [InlineData("Test Folder 123")]
    public async Task UpdateFolderDisplayName_WithDifferentNames_ShouldPassCorrectParameters(string displayName)
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        UpdateFolderDisplayNameRequest request = new()
        {
            DisplayName = displayName
        };

        _mockCommandService.Setup(x => x.UpdateFolderDisplayNameAsync(folderId, displayName))
            .ReturnsAsync(true);

        // Act
        bool result = await _controller.UpdateFolderDisplayName(folderId, request);

        // Assert
        Assert.True(result);
        _mockCommandService.Verify(
            x => x.UpdateFolderDisplayNameAsync(
                It.Is<Ulid>(id => id == folderId),
                It.Is<string>(name => name == displayName)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateFolderDisplayName_WithEmptyDisplayName_ShouldStillCallService()
    {
        // Arrange
        Ulid folderId = Ulid.NewUlid();
        UpdateFolderDisplayNameRequest request = new()
        {
            DisplayName = ""
        };

        _mockCommandService.Setup(x => x.UpdateFolderDisplayNameAsync(folderId, ""))
            .ReturnsAsync(false);

        // Act
        bool result = await _controller.UpdateFolderDisplayName(folderId, request);

        // Assert
        Assert.False(result);
        _mockCommandService.Verify(x => x.UpdateFolderDisplayNameAsync(folderId, ""), Times.Once);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_DisplayName_ShouldHaveRequiredAttribute()
    {
        // Arrange
        var property = typeof(UpdateFolderDisplayNameRequest).GetProperty(nameof(UpdateFolderDisplayNameRequest.DisplayName));

        // Act
        var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

        // Assert
        Assert.NotNull(requiredAttribute);
        Assert.Equal("Display name is required", requiredAttribute.ErrorMessage);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_DisplayName_ShouldHaveStringLengthAttribute()
    {
        // Arrange
        var property = typeof(UpdateFolderDisplayNameRequest).GetProperty(nameof(UpdateFolderDisplayNameRequest.DisplayName));

        // Act
        var stringLengthAttribute = property?.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault() as StringLengthAttribute;

        // Assert
        Assert.NotNull(stringLengthAttribute);
        Assert.Equal(250, stringLengthAttribute.MaximumLength);
        Assert.Equal(1, stringLengthAttribute.MinimumLength);
        Assert.Equal("Display name must be between 1 and 250 characters", stringLengthAttribute.ErrorMessage);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_WithExactly250Characters_ShouldPassValidation()
    {
        // Arrange
        string displayName = new string('a', 250);
        var request = new UpdateFolderDisplayNameRequest { DisplayName = displayName };
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_With251Characters_ShouldFailValidation()
    {
        // Arrange
        string displayName = new string('a', 251);
        var request = new UpdateFolderDisplayNameRequest { DisplayName = displayName };
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains("Display name must be between 1 and 250 characters", validationResults[0].ErrorMessage);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_WithEmptyString_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateFolderDisplayNameRequest { DisplayName = "" };
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(validationResults);
    }

    [Fact]
    public void UpdateFolderDisplayNameRequest_WithNull_ShouldFailValidation()
    {
        // Arrange
        var request = new UpdateFolderDisplayNameRequest { DisplayName = null! };
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(validationResults);
    }
}
