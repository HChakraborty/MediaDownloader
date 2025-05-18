namespace Downloader.Service.Model
{
    public class VideoFormat
    {
        public string FormatId { get; set; } = "";
        public string QualityLabel { get; set; } = ""; // 720p, 1080p, Audio only
        public string Extension { get; set; } = ""; // mp4, webm, etc.
        public long? FileSize { get; set; } // in bytes
        public bool HasVideo { get; set; }
        public bool HasAudio { get; set; }
    }
}
