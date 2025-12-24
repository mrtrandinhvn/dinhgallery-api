namespace dinhgallery_api.Infrastructures;

public interface IVideoProcessingService
{
    /// <summary>
    /// Optimizes a video file for streaming by moving moov atom to the beginning.
    /// This fixes iOS/iPhone playback issues where videos get stuck at the beginning.
    /// </summary>
    /// <param name="filePath">Full path to the video file</param>
    /// <returns>True if processing was successful or not needed, false if processing failed</returns>
    Task<bool> OptimizeForStreamingAsync(string filePath);

    /// <summary>
    /// Checks if a file is a video based on its extension
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>True if the file is a video type</returns>
    bool IsVideoFile(string filePath);
}
