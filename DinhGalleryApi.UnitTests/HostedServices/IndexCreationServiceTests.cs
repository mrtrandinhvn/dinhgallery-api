using dinhgallery_api.DbModels;
using dinhgallery_api.HostedServices;
using Moq;
using Redis.OM;
using Redis.OM.Contracts;

namespace DinhGalleryApi.UnitTests.HostedServices;

public class IndexCreationServiceTests
{
    [Fact]
    public async Task StartAsync_RecreatesFolderIndexBeforeCreatingIndexes()
    {
        var commands = new List<string>();
        var connection = new Mock<IRedisConnection>();
        connection
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<object[]>()))
            .Callback<string, object[]>((command, _) => commands.Add(command))
            .ReturnsAsync(default(RedisReply)!);

        var provider = new Mock<IRedisConnectionProvider>();
        provider.SetupGet(x => x.Connection).Returns(connection.Object);

        var service = new IndexCreationService(provider.Object);

        await service.StartAsync(CancellationToken.None);

        Assert.Equal("FT.DROPINDEX", commands[0]);
        Assert.Contains("FT.CREATE", commands);
        Assert.Equal(3, commands.Count);
        connection.Verify(
            x => x.ExecuteAsync("FT.DROPINDEX", It.IsAny<object[]>()),
            Times.Once);
    }
}
