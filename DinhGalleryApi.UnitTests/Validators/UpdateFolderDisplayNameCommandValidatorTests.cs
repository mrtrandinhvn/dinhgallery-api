using dinhgallery_api.BusinessObjects.Commands;
using dinhgallery_api.Controllers.GalleryEndpoints.Commands.UpdateFolderDisplayName;

namespace DinhGalleryApi.UnitTests.Validators;

public class UpdateFolderDisplayNameCommandValidatorTests
{
    private readonly UpdateFolderDisplayNameCommandValidator _validator;

    public UpdateFolderDisplayNameCommandValidatorTests()
    {
        _validator = new UpdateFolderDisplayNameCommandValidator();
    }

    [Fact]
    public void Validate_WithValidDisplayName_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = "Valid Folder Name"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithEmptyDisplayName_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Display name cannot be empty", result.Errors);
    }

    [Fact]
    public void Validate_WithWhitespaceDisplayName_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = "   "
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Display name cannot be empty", result.Errors);
    }

    [Fact]
    public void Validate_WithNullDisplayName_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = null!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Display name cannot be empty", result.Errors);
    }

    [Fact]
    public void Validate_WithDisplayNameExactly250Characters_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = new string('a', 250)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WithDisplayName251Characters_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = new string('a', 251)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("exceeds maximum length of 250 characters", result.Errors[0]);
    }

    [Fact]
    public void Validate_WithDisplayName300Characters_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = new string('a', 300)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("300", result.Errors[0]); // Should include actual length in error
    }

    [Theory]
    [InlineData("Folder 2026")]
    [InlineData("My Photos")]
    [InlineData("Test-Folder_123")]
    [InlineData("Special!@#$%^&*()")]
    public void Validate_WithVariousValidNames_ShouldReturnSuccess(string displayName)
    {
        // Arrange
        var command = new UpdateFolderDisplayNameCommand
        {
            FolderId = Ulid.NewUlid(),
            DisplayName = displayName
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}
