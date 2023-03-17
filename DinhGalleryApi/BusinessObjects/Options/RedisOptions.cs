namespace dinhgallery_api.BusinessObjects.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Password { get; set; }
}