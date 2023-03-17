using Redis.OM.Modeling;

namespace dinhgallery_api.DbModels;

[Document(Prefixes = new[] { TableName }, StorageType = StorageType.Json)]
public partial class FolderDbModel
{
    public const string TableName = "folder";

    [RedisIdField]
    [Indexed]
    public Ulid Id { get; set; }

    [Searchable(Sortable = true)]
    public string DisplayName { get; set; } = string.Empty;

    [Indexed(Sortable = true)]
    public DateTime CreatedAtUtc { get; set; }

    public string PhysicalFolderName { get; set; } = string.Empty;
}