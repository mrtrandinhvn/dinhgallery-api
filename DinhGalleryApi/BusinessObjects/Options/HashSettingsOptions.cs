namespace dinhgallery_api.BusinessObjects.Options;

public class HashSettingsOptions
{
    public const string SectionName = "HashSettings";

    public string FolderIdSalt { get; set; } = string.Empty;
}