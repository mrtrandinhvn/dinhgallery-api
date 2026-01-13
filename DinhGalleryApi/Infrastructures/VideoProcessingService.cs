using System.Diagnostics;

namespace dinhgallery_api.Infrastructures;

public class VideoProcessingService : IVideoProcessingService
{
    private readonly ILogger<VideoProcessingService> _logger;
    private static readonly string[] VideoExtensions = { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".m4v" };

    public VideoProcessingService(ILogger<VideoProcessingService> logger)
    {
        _logger = logger;
    }

    public bool IsVideoFile(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        return VideoExtensions.Contains(extension);
    }

    public async Task<bool> OptimizeForStreamingAsync(string filePath)
    {
        if (!IsVideoFile(filePath))
        {
            _logger.LogDebug("File '{FilePath}' is not a video file. Skipping optimization.", filePath);
            return true; // Not a video, no processing needed
        }

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Video file not found: '{FilePath}'. Cannot optimize.", filePath);
            return false;
        }

        _logger.LogInformation("Begin optimizing video for streaming: '{FilePath}'.", filePath);

        string extension = Path.GetExtension(filePath);
        string tempOutputPath = Path.ChangeExtension(filePath, ".tmp" + extension);

        try
        {
            // Run ffmpeg to move moov atom to beginning
            // -i input.mp4 = input file
            // -movflags faststart = move moov atom to beginning
            // -c copy = copy streams without re-encoding (fast, no quality loss)
            ProcessStartInfo startInfo = new()
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{filePath}\" -movflags faststart -c copy \"{tempOutputPath}\" -y",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? process = Process.Start(startInfo);
            if (process == null)
            {
                _logger.LogError("Failed to start ffmpeg process for file: '{FilePath}'.", filePath);
                return false;
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("ffmpeg failed for file '{FilePath}'. Exit code: {ExitCode}. Error: {Error}.",
                    filePath, process.ExitCode, error);

                // Clean up temp file if it exists
                if (File.Exists(tempOutputPath))
                {
                    File.Delete(tempOutputPath);
                }

                return false;
            }

            // Replace original file with optimized version
            File.Delete(filePath);
            File.Move(tempOutputPath, filePath);

            _logger.LogInformation("Successfully optimized video: '{FilePath}'.", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while optimizing video '{FilePath}'. Reason: {Reason}.", filePath, ex.Message);

            // Clean up temp file if it exists
            if (File.Exists(tempOutputPath))
            {
                try
                {
                    File.Delete(tempOutputPath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to clean up temporary file: '{TempFile}'.", tempOutputPath);
                }
            }

            return false;
        }
    }
}
