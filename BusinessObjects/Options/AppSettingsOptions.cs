namespace dinhgallery_api.BusinessObjects.Options;

public class StorageSettingsOptions
{
    public const string SectionName = "StorageSettings";

    public string FtpHost { get; set; } = string.Empty;
    public string FtpUsername { get; set; } = string.Empty;
    public string FtpPassword { get; set; } = string.Empty;
    public string StorageServiceBaseUrl { get; set; } = string.Empty;
}