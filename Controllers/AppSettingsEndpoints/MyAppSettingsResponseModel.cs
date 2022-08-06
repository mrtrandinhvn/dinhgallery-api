namespace dinhgallery_api.Controllers.AppSettingsEndpoints
{
    public class MyAppSettingsResponseModel
    {
        public string? FtpHost { get; set; }
        public string? FtpUsername { get; set; }
        public string? FtpPassword { get; set; }
        public string? StorageServiceBaseUrl { get; set; }
    }
}