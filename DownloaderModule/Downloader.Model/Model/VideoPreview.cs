using YoutubeDLSharp.Metadata;

namespace Downloader.Service.Model
{
    public class VideoPreview
    {
        public string Title { get; set; } = "";
        public string Uploader { get; set; } = "";
        public DateTime? UploadDate { get; set; }
        public float? Duration { get; set; } // in seconds
        public string Thumbnail { get; set; } = "";

        public ThumbnailData[] Thumbnails { get; set; } = [];
        public string Description { get; set; } = "";
        public string WebpageUrl { get; set; } = "";
        public string Format { get; set; } = "";
    }
}
