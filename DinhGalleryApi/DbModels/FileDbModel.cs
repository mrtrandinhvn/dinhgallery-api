using Redis.OM.Modeling;

namespace dinhgallery_api.DbModels
{
    [Document(Prefixes = new[] { TableName }, StorageType = StorageType.Json)]
    public class FileDbModel
    {
        public const string TableName = "file";

        [RedisIdField]
        [Indexed]
        public Ulid Id { get; set; }

        [Searchable(Sortable = true)]
        public string DisplayName { get; set; } = string.Empty;

        public string DownloadUri { get; set; } = string.Empty;

        [Indexed(Sortable = true)]
        public DateTime CreatedAtUtc { get; set; }

        [Indexed]
        public Ulid FolderId { get; set; }
    }
}