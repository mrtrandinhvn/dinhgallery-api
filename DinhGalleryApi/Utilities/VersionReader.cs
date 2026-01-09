namespace dinhgallery_api.Utilities;

public static class VersionReader
{
    private static string? _cachedVersion;

    /// <summary>
    /// Reads the current version from the VERSION file in the project root.
    /// </summary>
    public static string GetCurrentVersion(string contentRootPath)
    {
        if (_cachedVersion != null)
        {
            return _cachedVersion;
        }

        string versionFilePath = Path.Combine(contentRootPath, "VERSION");

        if (!File.Exists(versionFilePath))
        {
            return "unknown";
        }

        try
        {
            string[] lines = File.ReadAllLines(versionFilePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("CURRENT="))
                {
                    _cachedVersion = line.Substring("CURRENT=".Length).Trim();
                    return _cachedVersion;
                }
            }
        }
        catch
        {
            return "unknown";
        }

        return "unknown";
    }
}
